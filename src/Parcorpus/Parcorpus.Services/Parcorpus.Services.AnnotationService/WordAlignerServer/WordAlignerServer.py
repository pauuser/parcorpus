from concurrent import futures
from simalign import SentenceAligner
import nltk

nltk.download('punkt')

import grpc
import aligner_pb2
import aligner_pb2_grpc

aligner = SentenceAligner(model="bert", token_type="bpe", matching_methods="i", device="cpu")

delims = [';', '.', '\'', '\"', ':', '!', '?', ',', '/', '\\', '|', '(', ')', '*', '«', '»']


class WordAlignerServer(aligner_pb2_grpc.WordAlignerServicer):
    def AlignWords(self, request, context):
        src_tokens = nltk.word_tokenize(request.sourceText, language=request.sourceLanguageShortName.lower())
        tgr_tokens = nltk.word_tokenize(request.targetText, language=request.targetLanguageShortName.lower())

        alignments = aligner.get_word_aligns(src_tokens, tgr_tokens)

        dict = {}
        for elem in alignments['itermax']:
            if elem[0] not in dict:
                dict[elem[0]] = [elem[1]]
            else:
                dict[elem[0]].append(elem[1])

        items = dict.items()
        values_dict = {}
        for item in items:
            item = list(item)
            item[1] = tuple(item[1])
            if item[1] not in values_dict:
                values_dict[item[1]] = [item[0]]
            else:
                values_dict[item[1]].append(item[0])

        result_dict = {}
        for item in values_dict.items():
            item = list(item)
            item[1] = tuple(item[1])
            result_dict[item[1]] = item[0]

        result = map(lambda elem: (" ".join(list(map(lambda x: src_tokens[x], elem[0]))),
                                   " ".join(list(map(lambda x: tgr_tokens[x], elem[1])))), result_dict.items())

        result = list(result)
        for x in result:
            if x[0].strip() in delims or x[1].strip() in delims:
                result.remove(x)

        return aligner_pb2.AlignmentReply(words=[aligner_pb2.AlignmentReply.AlignedWord(
            sourceWord=x[0], targetWord=x[1]) for x in list(result)])


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    aligner_pb2_grpc.add_WordAlignerServicer_to_server(
        WordAlignerServer(), server
    )
    server.add_insecure_port("[::]:50051")
    server.start()
    server.wait_for_termination()


if __name__ == "__main__":
    print("Server is running!")
    serve()
