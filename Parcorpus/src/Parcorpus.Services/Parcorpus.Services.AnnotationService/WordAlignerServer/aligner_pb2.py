# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: aligner.proto
"""Generated protocol buffer code."""
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import symbol_database as _symbol_database
from google.protobuf.internal import builder as _builder
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\raligner.proto\x12\x07\x61ligner\"|\n\x10SentencesRequest\x12\x12\n\nsourceText\x18\x01 \x01(\t\x12\x12\n\ntargetText\x18\x02 \x01(\t\x12\x1f\n\x17sourceLanguageShortName\x18\x03 \x01(\t\x12\x1f\n\x17targetLanguageShortName\x18\x04 \x01(\t\"{\n\x0e\x41lignmentReply\x12\x32\n\x05words\x18\x01 \x03(\x0b\x32#.aligner.AlignmentReply.AlignedWord\x1a\x35\n\x0b\x41lignedWord\x12\x12\n\nsourceWord\x18\x01 \x01(\t\x12\x12\n\ntargetWord\x18\x02 \x01(\t2Q\n\x0bWordAligner\x12\x42\n\nAlignWords\x12\x19.aligner.SentencesRequest\x1a\x17.aligner.AlignmentReply\"\x00\x62\x06proto3')

_globals = globals()
_builder.BuildMessageAndEnumDescriptors(DESCRIPTOR, _globals)
_builder.BuildTopDescriptorsAndMessages(DESCRIPTOR, 'aligner_pb2', _globals)
if _descriptor._USE_C_DESCRIPTORS == False:

  DESCRIPTOR._options = None
  _globals['_SENTENCESREQUEST']._serialized_start=26
  _globals['_SENTENCESREQUEST']._serialized_end=150
  _globals['_ALIGNMENTREPLY']._serialized_start=152
  _globals['_ALIGNMENTREPLY']._serialized_end=275
  _globals['_ALIGNMENTREPLY_ALIGNEDWORD']._serialized_start=222
  _globals['_ALIGNMENTREPLY_ALIGNEDWORD']._serialized_end=275
  _globals['_WORDALIGNER']._serialized_start=277
  _globals['_WORDALIGNER']._serialized_end=358
# @@protoc_insertion_point(module_scope)
