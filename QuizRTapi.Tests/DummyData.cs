using System;
using Xunit;
using Moq;
using System.Linq;
using System.Collections.Generic;
using QuizRT.Models;
using Microsoft.AspNetCore.Mvc;
using QuizRTapi.Controllers;
namespace QuizRTapi.Tests
{
    public class DummyData
    {
        public IEnumerable<QuestionGeneration> DummyMock(){
            QuestionGeneration questionGeneration = new QuestionGeneration
            {
                Text = "string",
                CategoryId = "string",
                CategoryName = "string",
                TopicId = "string",
                TopicName = "string"
            };
            yield return questionGeneration;
        }
    }
}
