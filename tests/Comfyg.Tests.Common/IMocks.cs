using Moq;

namespace Comfyg.Tests.Common;

public interface IMocks
{
    void ResetMocks();

    void Mock<T>(Action<Mock<T>> mockProvider) where T : class;

    Mock<T> GetMock<T>() where T : class;
}
