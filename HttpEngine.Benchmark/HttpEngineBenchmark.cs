using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using HttpEngine.Processing;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace HttpEngine.Benchmark
{
    [CoreJob]
    [IterationTime(500)]
    [MemoryDiagnoser]
    [InliningDiagnoser]
    [TailCallDiagnoser]
    public class HttpEngineBenchmark
    {
        private byte[] _smallMessage;
        private byte[] _mediumMessage;
        private byte[] _bigMessage;
        private byte[] _fullExample;
        private string _bigMessageString;

        private IHttpMessageParser _messageBuilder;

        public HttpEngineBenchmark()
        {
#if DEBUG
            GlobalSetup();
#endif
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _messageBuilder = new HttpMessageParser(new HeaderReader(), new DefaultHeaderParser(new HeaderParametersParser()),
                new HttpHeaderSlicer(), new StartLineParser());
            _smallMessage = File.ReadAllBytes("HttpMessageSmall.txt");
            _mediumMessage = File.ReadAllBytes("HttpMessageMedium.txt");
            _bigMessage = File.ReadAllBytes("HttpMessageBig.txt");
            _fullExample = File.ReadAllBytes("HttpExample.txt");
            _bigMessageString = Encoding.UTF8.GetString(_bigMessage);
        }

        [Benchmark]
        public object SmallMessage()
        {
            var message = _messageBuilder.ParseMessage(_smallMessage);
            return message;
        }

        [Benchmark]
        public object MediumMessage()
        {
            var message = _messageBuilder.ParseMessage(_mediumMessage);
            return message;
        }

        [Benchmark]
        public object BigMessage()
        {
            var message = _messageBuilder.ParseMessage(_bigMessage);
            return message;
        }

        [Benchmark]
        public object FullExample()
        {
            var message = _messageBuilder.ParseMessage(_fullExample);
            return message;
        }
    }
}
