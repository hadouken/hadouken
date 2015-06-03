using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.JsonRpc;
using Hadouken.Common.Text;
using Hadouken.Core.JsonRpc;
using Hadouken.Core.Tests.Fakes;
using NSubstitute;
using Xunit;

namespace Hadouken.Core.Tests.Unit.JsonRpc {
    public class RequestHandlerTests {
        private static RequestHandler CreateRequestHandler(params IJsonRpcService[] services) {
            var cacheBuilder = new MethodCacheBuilder(services);
            var nameResolver = new ByNameResolver(new JsonSerializer());
            var positionResolver = new ByPositionResolver(new JsonSerializer());
            var nullResolver = new NullResolver();

            return new RequestHandler(cacheBuilder,
                new IParameterResolver[] {nameResolver, positionResolver, nullResolver});
        }

        public class TheConstructor {
            [Fact]
            public void Throws_ArgumentNullException_If_CacheBuilder_Is_Null() {
                // Given, When
                var exception = Record.Exception(() => new RequestHandler(null, Enumerable.Empty<IParameterResolver>()));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("cacheBuilder", ((ArgumentNullException) exception).ParamName);
            }

            [Fact]
            public void Throws_ArgumentNullException_If_ParameterResolvers_Is_Null() {
                // Given, When
                var exception = Record.Exception(() => new RequestHandler(Substitute.For<IMethodCacheBuilder>(), null));

                // Then
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("parameterResolvers", ((ArgumentNullException) exception).ParamName);
            }
        }

        public class TheHandleMethod {
            [Fact]
            public void Returns_Method_Not_Found_Error_For_Nonexistent_Method_Name() {
                // Given
                var request = new JsonRpcRequest {Id = 1, MethodName = "nonexistent"};
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var response = (JsonRpcErrorResponse) handler.Handle(request);

                // Then
                Assert.Equal("Method not found", response.Error.Message);
            }

            [Fact]
            public void Returns_Invalid_Params_Error_For_String_As_Parameter() {
                // Given
                var request = new JsonRpcRequest {Id = 1, MethodName = "test", Parameters = "invalid"};
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var response = (JsonRpcErrorResponse) handler.Handle(request);

                // Then
                Assert.Equal("Invalid params", response.Error.Message);
            }

            [Fact]
            public void Returns_Correct_Result_For_Valid_Request() {
                // Given
                var handler = CreateRequestHandler(new JsonRpcServiceFake());
                var request = new JsonRpcRequest {Id = 1, MethodName = "test"};

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.Equal(42, result.Result);
            }

            [Fact]
            public void Returns_Correct_Result_For_Positional_Array_And_Object_Parameters() {
                // Given
                var handler = CreateRequestHandler(new JsonRpcServiceFake());
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.array+object",
                    Parameters = new object[] {new[] {1, 1, 3}, new TestDto()}
                };

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.True((bool) result.Result);
            }

            [Fact]
            public void Returns_Correct_Result_For_Named_Array_And_Object_Parameters() {
                // Given
                var handler = CreateRequestHandler(new JsonRpcServiceFake());
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.array+object",
                    Parameters = new Dictionary<string, object> {
                        {"array", new[] {1, 2, 3}},
                        {"obj", new TestDto()}
                    }
                };

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.True((bool) result.Result);
            }

            [Fact]
            public void Returns_Correct_Result_For_Positional_String_Parameter() {
                // Given
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.string",
                    Parameters = new object[] {"test"}
                };
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.True((bool) result.Result);
            }

            [Fact]
            public void Returns_Correct_Result_For_Named_String_Parameter() {
                // Given
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.string",
                    Parameters = new Dictionary<string, object> {
                        {"val", "test"}
                    }
                };
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.True((bool) result.Result);
            }

            [Fact]
            public void Returns_Correct_Result_For_Positional_String_And_Boolean_Parameters() {
                // Given
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.string+bool",
                    Parameters = new object[] {"test", false}
                };
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.True((bool) result.Result);
            }

            [Fact]
            public void Returns_Correct_Result_For_Named_String_And_Boolean_Parameters() {
                // Given
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.string+bool",
                    Parameters = new Dictionary<string, object> {
                        {"str", "test"},
                        {"b", true}
                    }
                };
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.True((bool) result.Result);
            }

            [Fact]
            public void Returns_Correct_Result_For_Positional_Array_Parameter() {
                // Given
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.array",
                    Parameters = new object[] {new[] {"1", "2", "3", "4"}}
                };
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.Equal(4, ((string[]) result.Result).Length);
            }

            [Fact]
            public void Returns_Correct_Result_For_Named_Array_Parameter() {
                // Given
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.array",
                    Parameters = new Dictionary<string, object> {
                        {"arr", new[] {"1", "2", "3", "4"}}
                    }
                };
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.Equal(4, ((string[]) result.Result).Length);
            }

            [Fact]
            public void Returns_Correct_Result_For_Method_Accepting_No_Arguments() {
                // Given
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.noargs"
                };
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.True((bool) result.Result);
            }

            [Fact]
            public void Returns_Correct_Result_For_Method_Returning_Void_Given_Positional_Parameters() {
                // Given
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.void",
                    Parameters = new[] {1, 2}
                };
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.Null(result.Result);
            }

            [Fact]
            public void Returns_Correct_Result_For_Method_Returning_Void_Given_Named_Parameters() {
                // Given
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.void",
                    Parameters = new Dictionary<string, object> {
                        {"i", 1},
                        {"j", 2}
                    }
                };
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.Null(result.Result);
            }

            [Fact]
            public void Returns_Correct_Error_Code_For_Method_Throwing_JsonRpcException() {
                // Given
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.throw"
                };
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var result = (JsonRpcErrorResponse) handler.Handle(request);

                // Then
                Assert.Equal(1000, result.Error.Code);
            }

            [Fact]
            public void Returns_Correct_Enum_For_Dto_With_Enum_Property() {
                // Given
                var request = new JsonRpcRequest {
                    Id = 1,
                    MethodName = "test.objectEnum",
                    Parameters = new[] {new TestDto {Enum = TestEnum.Two}}
                };
                var handler = CreateRequestHandler(new JsonRpcServiceFake());

                // When
                var result = (JsonRpcSuccessResponse) handler.Handle(request);

                // Then
                Assert.Equal(TestEnum.Two, (TestEnum) result.Result);
            }
        }
    }
}