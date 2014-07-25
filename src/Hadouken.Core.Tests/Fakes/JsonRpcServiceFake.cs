using Hadouken.Common.JsonRpc;

namespace Hadouken.Core.Tests.Fakes
{
    public class JsonRpcServiceFake : IJsonRpcService
    {
        [JsonRpcMethod("test")]
        public int Test()
        {
            return 42;
        }

        [JsonRpcMethod("test.array+object")]
        public bool ArrayObject(int[] array, TestDto obj)
        {
            return true;
        }

        [JsonRpcMethod("test.objectEnum")]
        public TestEnum TestEnum(TestDto dto)
        {
            return dto.Enum;
        }

        [JsonRpcMethod("test.string")]
        public bool String(string val)
        {
            return true;
        }

        [JsonRpcMethod("test.string+bool")]
        public bool StringBool(string str, bool b)
        {
            return true;
        }

        [JsonRpcMethod("test.array")]
        public string[] Array(string[] arr)
        {
            return arr;
        }

        [JsonRpcMethod("test.noargs")]
        public bool NoArgs()
        {
            return true;
        }

        [JsonRpcMethod("test.void")]
        public void VoidMethod(int i, int j)
        {
            var nop = i + j;
        }

        [JsonRpcMethod("test.throw")]
        public void Throw()
        {
            throw new JsonRpcException(1000);
        }
    }

    public class TestDto
    {
        public string Foo { get; set; }

        public int Bar { get; set; }

        public TestEnum Enum { get; set; }
    }

    public enum TestEnum
    {
        One,
        Two
    }
}
