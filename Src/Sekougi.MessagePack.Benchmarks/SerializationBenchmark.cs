using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using MsgPack.Serialization;
using System.IO;
using MsgPack;
using Sekougi.MessagePack.Buffers;
using TupleData = System.ValueTuple<int, double, long>;


namespace Sekougi.MessagePack.Benchmarks
{
    [MemoryDiagnoser]
    public class SerializationBenchmark
    {
        private string _shortString;
        private string _averageString;
        private string _largeString;
        private List<TupleData> _tuples;
        private TupleData _tuple;
        private int[] _intArray;
        
        private Stream _cliStream;
        private Packer _packerCli;
        private MemoryStream _streamCliInt;
        private MemoryStream _streamCliTuple;
        private MemoryStream _streamCliFloat;
        private MemoryStream _streamCliString;
        private MemoryStream _streamCliArrayInt;
        private MessagePackSerializer<int> _serializerIntCli;
        private MessagePackSerializer<double> _serializerDoubleCli;
        private MessagePackSerializer<float> _serializerFloatCli;
        private MessagePackSerializer<string> _serializerStringCli;
        private MessagePackSerializer<int[]> _serializerArrayIntCli;
        private MessagePackSerializer<List<TupleData>> _serializerTupleArrayCli;
        private MessagePackSerializer<TupleData> _serializerTupleCli;

        private MessagePackBuffer _sekougiDeserializationBufferInt;
        private MessagePackBuffer _sekougiDeserializationBufferFloat;
        private MessagePackBuffer _sekougiDeserializationBufferString;
        private MessagePackBuffer _sekougiDeserializationBufferIntArray;
        private MessagePackBuffer _sekougiDeserializationBufferTuple;
        private MessagePackBuffer _sekougiBuffer;
        private MessagePackWriter _writerSekougi;
        private MessagePackReader _readerIntArraySekougi;
        private MessagePackReader _readerIntSekougi;
        private MessagePackReader _readerFloatSekougi;
        private MessagePackReader _readerStringSekougi;
        private MessagePackReader _readerTupleSekougi;
        private Serializers.MessagePackSerializer<int> _serializerIntSekougi;
        private Serializers.MessagePackSerializer<double> _serializerDoubleSekougi;
        private Serializers.MessagePackSerializer<float> _serializerFloatSekougi;
        private Serializers.MessagePackSerializer<string> _serializerStringSekougi;
        private Serializers.MessagePackSerializer<int[]> _serializerArrayIntSekougi;
        private Serializers.MessagePackSerializer<List<TupleData>> _serializerTupleArraySekougi;
        private Serializers.MessagePackSerializer<TupleData> _serializerTupleSekougi;

        
        [GlobalSetup]
        public void Setup()
        {
            var size = 100000;
            _shortString = new string(new char[10]);
            _averageString = new string(new char[10000]);
            _largeString = new string(new char[int.MaxValue / 10]);
            _tuple = (int.MaxValue, double.MaxValue, int.MaxValue);
            _intArray = new int[size];
            
            _tuples = new List<TupleData>(size);
            for (var i = 0; i < size; i++)
            {
                var tuple = new TupleData(Int32.MaxValue - i, double.MaxValue - i, long.MaxValue - 1);
                _tuples.Add(tuple);
            }
            
            _serializerIntCli =  MessagePackSerializer.Get<int>();
            _serializerDoubleCli = MessagePackSerializer.Get<double>();
            _serializerFloatCli = MessagePackSerializer.Get<float>();
            _serializerStringCli = MessagePackSerializer.Get<string>();
            _serializerArrayIntCli = MessagePackSerializer.Get<int[]>();
            _serializerTupleArrayCli = MessagePackSerializer.Get<List<TupleData>>();
            _serializerTupleCli = MessagePackSerializer.Get<TupleData>();
            
            _serializerIntSekougi = MessagePackSerializersRepository.Get<int>();
            _serializerDoubleSekougi = MessagePackSerializersRepository.Get<double>();
            _serializerFloatSekougi = MessagePackSerializersRepository.Get<float>();
            _serializerStringSekougi = MessagePackSerializersRepository.Get<string>();
            _serializerArrayIntSekougi = MessagePackSerializersRepository.Get<int[]>();
            _serializerTupleArraySekougi = MessagePackSerializersRepository.Get<List<TupleData>>();
            _serializerTupleSekougi = MessagePackSerializersRepository.Get<TupleData>();
            
            var stringToSerialize = new string(new char[size]);
            var intArrayToSerialize = new int[size];
            
            _streamCliTuple = new MemoryStream();
            _serializerTupleCli.Pack(_streamCliTuple, _tuple);
            
            _streamCliInt = new MemoryStream();
            _serializerIntCli.Pack(_streamCliInt, int.MaxValue);
            
            _streamCliFloat = new MemoryStream();
            _serializerFloatCli.Pack(_streamCliFloat, float.MaxValue);
            
            _streamCliString = new MemoryStream();
            _serializerStringCli.Pack(_streamCliString, new string(stringToSerialize));
            
            _streamCliArrayInt = new MemoryStream();
            _serializerArrayIntCli.Pack(_streamCliArrayInt, intArrayToSerialize);
            
            
            _sekougiDeserializationBufferIntArray = new MessagePackBuffer();
            var writer = new MessagePackWriter(_sekougiDeserializationBufferIntArray);
            _serializerArrayIntSekougi.Serialize(intArrayToSerialize, writer);
            
            _sekougiDeserializationBufferTuple = new MessagePackBuffer();
            writer = new MessagePackWriter(_sekougiDeserializationBufferTuple);
            _serializerTupleSekougi.Serialize(_tuple, writer);
            
            _sekougiDeserializationBufferInt = new MessagePackBuffer();
            writer = new MessagePackWriter(_sekougiDeserializationBufferInt);
            _serializerIntSekougi.Serialize(int.MaxValue, writer);
            
            _sekougiDeserializationBufferFloat = new MessagePackBuffer();
            writer = new MessagePackWriter(_sekougiDeserializationBufferFloat);
            _serializerFloatSekougi.Serialize(float.MaxValue, writer);
            
            _sekougiDeserializationBufferString = new MessagePackBuffer();
            writer = new MessagePackWriter(_sekougiDeserializationBufferString);
            _serializerStringSekougi.Serialize(new string(stringToSerialize), writer);
        }
        
        [IterationSetup]
        public void IterationSetup()
        {
            const int BUFFER_CAPACITY = 1024000;
            
            SetupSekougi();
            SetupCli();

            void SetupSekougi()
            {
                _sekougiBuffer = new MessagePackBuffer(BUFFER_CAPACITY);
                _writerSekougi = new MessagePackWriter(_sekougiBuffer);
                
                _readerIntArraySekougi = new MessagePackReader(_sekougiDeserializationBufferIntArray);
                _readerIntSekougi = new MessagePackReader(_sekougiDeserializationBufferInt);
                _readerFloatSekougi = new MessagePackReader(_sekougiDeserializationBufferFloat);
                _readerStringSekougi = new MessagePackReader(_sekougiDeserializationBufferString);
                _readerTupleSekougi = new MessagePackReader(_sekougiDeserializationBufferTuple);
            
                _sekougiDeserializationBufferIntArray.Position = 0;
                _sekougiDeserializationBufferString.Position = 0;
                _sekougiDeserializationBufferFloat.Position = 0;
                _sekougiDeserializationBufferInt.Position = 0;
                _sekougiDeserializationBufferTuple.Position = 0;
            }

            void SetupCli()
            {
                _cliStream = new MemoryStream(BUFFER_CAPACITY);
                _packerCli = Packer.Create(_cliStream);  
                
                _streamCliInt.Position = 0;
                _streamCliFloat.Position = 0;
                _streamCliString.Position = 0;
                _streamCliArrayInt.Position = 0;
                _streamCliTuple.Position = 0;
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _cliStream.Dispose();
            _sekougiBuffer.Dispose();
        }
        
        [Benchmark]
        [Arguments(1)]
        [Arguments(100)]
        public void SerializeTupleCli(int count)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerTupleCli.PackTo(_packerCli, _tuple);
            }
        }
        
        [Benchmark]
        [Arguments(1)]
        [Arguments(100)]
        public void SerializeTupleSekougi(int count)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerTupleSekougi.Serialize(_tuple, _writerSekougi);
            }
        }

        [Benchmark]
        [Arguments(1, int.MaxValue)]
        [Arguments(100, int.MaxValue)]
        [Arguments(1, 255)]
        [Arguments(100, 255)]
        public void SerializeIntCli(int count, int value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerIntCli.PackTo(_packerCli, value);
            }
        }
        
        [Benchmark]
        [Arguments(1, int.MaxValue)]
        [Arguments(100, int.MaxValue)]
        [Arguments(1, 255)]
        [Arguments(100, 255)]
        public void SerializeIntSekougi(int count, int value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerIntSekougi.Serialize(value, _writerSekougi);
            }
        }
        
        [Benchmark]
        [Arguments(1, double.MaxValue)]
        [Arguments(100, double.MaxValue)]
        [Arguments(100, 255.1d)]
        public void SerializeDoubleCli(int count, double value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerDoubleCli.PackTo(_packerCli, value);
            }
        }
        
        [Benchmark]
        [Arguments(1, double.MaxValue)]
        [Arguments(100, double.MaxValue)]
        [Arguments(100, 255.1d)]
        public void SerializeDoubleSekougi(int count, double value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerDoubleSekougi.Serialize(value, _writerSekougi);
            }
        }
        
        [Benchmark]
        [Arguments(1, float.MaxValue)]
        [Arguments(100, float.MaxValue)]
        [Arguments(100, 255.1f)]
        public void SerializeFloatCli(int count, float value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerFloatCli.PackTo(_packerCli, value);
            }
        }
        
        [Benchmark]
        [Arguments(1, float.MaxValue)]
        [Arguments(100, float.MaxValue)]
        [Arguments(100, 255.1f)]
        public void SerializeFloatSekougi(int count, float value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerFloatSekougi.Serialize(value, _writerSekougi);
            }
        }

        [Benchmark]
        public void DeSerializeIntCli() => _serializerIntCli.Unpack(_streamCliInt);
        
        [Benchmark]
        public void DeSerializeIntSekougi() => _serializerIntSekougi.Deserialize(_readerIntSekougi);
        
        [Benchmark]
        public void DeSerializeFloatCli() => _serializerFloatCli.Unpack(_streamCliFloat);
        
        [Benchmark]
        public void DeSerializeFloatSekougi() => _serializerFloatSekougi.Deserialize(_readerFloatSekougi);

        [Benchmark]
        public void DeSerializeStringCli() => _serializerStringCli.Unpack(_streamCliString);
        
        [Benchmark]
        public void DeSerializeStringSekougi() => _serializerStringSekougi.Deserialize(_readerStringSekougi);

        [Benchmark]
        public void DeSerializeIntArrayCli() => _serializerArrayIntCli.Unpack(_streamCliArrayInt);
        
        [Benchmark]
        public void DeSerializeIntArraySekougi() => _serializerArrayIntSekougi.Deserialize(_readerIntArraySekougi);
        
        [Benchmark]
        public void DeSerializeTupleCli() => _serializerTupleCli.Unpack(_streamCliTuple);
        
        [Benchmark]
        public void DeSerializeTupleSekougi() => _serializerTupleSekougi.Deserialize(_readerTupleSekougi);
        
        [Benchmark]
        public void SerializeIntArrayCli() => _serializerArrayIntCli.Pack(_cliStream, _intArray);
       
        [Benchmark]
        public void SerializeIntArraySekougi() => _serializerArrayIntSekougi.Serialize(_intArray, _sekougiBuffer);
        
        [Benchmark]
        public void SerializeTupleArrayCli() => _serializerTupleArrayCli.PackTo(_packerCli, _tuples);

        [Benchmark]
        public void SerializeTupleArraySekougi() => _serializerTupleArraySekougi.Serialize(_tuples, _writerSekougi);
        
        [Benchmark]
        public void SerializeShortStringCli() => _serializerStringCli.PackTo(_packerCli, _shortString);
        
        [Benchmark]
        public void SerializeAverageStringCli() => _serializerStringCli.PackTo(_packerCli, _averageString);
        
        [Benchmark]
        public void SerializeLargeStringCli() => _serializerStringCli.PackTo(_packerCli, _largeString);
        
        [Benchmark]
        public void SerializeShortStringSekougi() => _serializerStringSekougi.Serialize(_shortString, _writerSekougi);
        
        [Benchmark]
        public void SerializeAverageStringSekougi() => _serializerStringSekougi.Serialize(_averageString, _writerSekougi);
        
        [Benchmark]
        public void SerializeLargeStringSekougi() => _serializerStringSekougi.Serialize(_largeString, _writerSekougi);
    }
}