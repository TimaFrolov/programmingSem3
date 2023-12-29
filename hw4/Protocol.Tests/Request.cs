namespace SimpleFTP;

using Utils;

public class RequestTests
{
    static (Request, string)[] requests =
    {
        (new Request.List("dir1"), "1 dir1"),
        (new Request.Get("file1"), "2 file1"),
    };

    [TestCaseSource(nameof(requests))]
    public void TestToString((Request request, string expected) testCase)
    {
        Assert.That(testCase.request.ToString(), Is.EqualTo(testCase.expected));
    }

    [TestCaseSource(nameof(requests))]
    public void TestTryFrom((Request request, string _) testCase)
    {
        var request = Request.TryFrom(testCase.Item2);
        Assert.That(request, Is.TypeOf<Utils.Option<Request>.Some>());
        var req = request.Unwrap();
        Assert.That(req, Is.EqualTo(testCase.request));
    }
}
