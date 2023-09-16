from google.protobuf.internal import containers as _containers
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Iterable as _Iterable, Mapping as _Mapping, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class SentencesRequest(_message.Message):
    __slots__ = ["sourceText", "targetText", "sourceLanguageShortName", "targetLanguageShortName"]
    SOURCETEXT_FIELD_NUMBER: _ClassVar[int]
    TARGETTEXT_FIELD_NUMBER: _ClassVar[int]
    SOURCELANGUAGESHORTNAME_FIELD_NUMBER: _ClassVar[int]
    TARGETLANGUAGESHORTNAME_FIELD_NUMBER: _ClassVar[int]
    sourceText: str
    targetText: str
    sourceLanguageShortName: str
    targetLanguageShortName: str
    def __init__(self, sourceText: _Optional[str] = ..., targetText: _Optional[str] = ..., sourceLanguageShortName: _Optional[str] = ..., targetLanguageShortName: _Optional[str] = ...) -> None: ...

class AlignmentReply(_message.Message):
    __slots__ = ["words"]
    class AlignedWord(_message.Message):
        __slots__ = ["sourceWord", "targetWord"]
        SOURCEWORD_FIELD_NUMBER: _ClassVar[int]
        TARGETWORD_FIELD_NUMBER: _ClassVar[int]
        sourceWord: str
        targetWord: str
        def __init__(self, sourceWord: _Optional[str] = ..., targetWord: _Optional[str] = ...) -> None: ...
    WORDS_FIELD_NUMBER: _ClassVar[int]
    words: _containers.RepeatedCompositeFieldContainer[AlignmentReply.AlignedWord]
    def __init__(self, words: _Optional[_Iterable[_Union[AlignmentReply.AlignedWord, _Mapping]]] = ...) -> None: ...
