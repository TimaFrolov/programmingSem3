namespace SimpleFTP.tests;

public class ResponseTests
{
    static (Response, string)[] responses =
    {
        (
            new Response.List(
                new (string, bool)[]
                {
                    ("dir1", true),
                    ("file1", false),
                    ("dir2", true),
                    ("file2", false)
                }
            ),
            "4 dir1 true file1 false dir2 true file2 false"
        ),
        (new Response.Get("Test string"), $"{"Test string".Length} Test string"),
        (new Response.Error("Test error"), "Error: Test error"),
        (Response.UnknownRequest, "Unknown request")
    };

    [TestCaseSource(nameof(responses))]
    public void TestToString((Response response, string expected) testCase)
    {
        Assert.That(testCase.response.ToString(), Is.EqualTo(testCase.expected));
    }

    [Test]
    public void TestParseList()
    {
        var response = Response.List.TryParse("4 dir1 true file1 false dir2 true file2 false");
        Assert.That(response, Is.TypeOf<Utils.Option<Response.List>.Some>());
        var list = response.Unwrap();
        Assert.That(
            list.list,
            Is.EquivalentTo(
                new (string, bool)[]
                {
                    ("dir1", true),
                    ("file1", false),
                    ("dir2", true),
                    ("file2", false)
                }
            )
        );
    }

    [Test]
    public void TestParseListIncorrect()
    {
        var response = Response.List.TryParse("1 dir1");
        Assert.That(response, Is.EqualTo(Utils.Option<Response.List>.None));
    }

    [Test]
    public void TestParseListIncorrectBool()
    {
        var response = Response.List.TryParse("1 dir1 maybe");
        Assert.That(response, Is.EqualTo(Utils.Option<Response.List>.None));
    }

    [Test]
    public async Task TestParseGet()
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write($"{"Test string".Length} Test string");
        writer.Flush();
        stream.Position = 0;
        var response = await Response.Get.TryParseAsync(new StreamReader(stream));
        Assert.That(response, Is.TypeOf<Utils.Option<Response.Get>.Some>());
        var get = response.Unwrap();
        Assert.That(get.data, Is.EqualTo("Test string"));
    }

    [Test]
    public async Task TestParseGetIncorrect()
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write($"{"Test string".Length} Test");
        writer.Flush();
        stream.Position = 0;
        var response = await Response.Get.TryParseAsync(new StreamReader(stream));
        Assert.That(response, Is.EqualTo(Utils.Option<Response.Get>.None));
    }
}
