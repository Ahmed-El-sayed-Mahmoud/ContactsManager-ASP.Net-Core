using AutoFixture;
using FluentAssertions;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using Xunit.Abstractions;

namespace TestProject
{
    public class PersonsControllerIntegrationTest:IClassFixture<CustomWebAppFactory>
    {
        private readonly HttpClient? _httpClient;
        private readonly ITestOutputHelper _testOutputHelper;
        public PersonsControllerIntegrationTest(CustomWebAppFactory customWebAppFactory,ITestOutputHelper outputHelper)
        {
            _httpClient = customWebAppFactory.CreateClient();
            _testOutputHelper = outputHelper;
        }
        [Fact]
        public async Task Index_ReturnView()
        {
           HttpResponseMessage httpResponseMessage= await _httpClient.GetAsync("./Index");

           // httpResponseMessage.Should().BeSuccessful();

            string resposneBody = await httpResponseMessage.Content.ReadAsStringAsync();
            HtmlDocument htmlDocument= new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(resposneBody);
            HtmlNode htmlNode = htmlDocument.DocumentNode;
            _testOutputHelper.WriteLine(resposneBody);
            htmlNode.QuerySelectorAll(".persons").Should().NotBeEmpty();
        }
    }
}
