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
    public class UnitTest1
    {
        [Fact]
        public void GetTemplate(){
            // DummyData DD = new DummyData();
            // List<GingerNoteC> dummy = DD.DummyMock();  // Arrange
            
            Mock<IQuizRTRepo> MockRepository = new Mock<IQuizRTRepo>(); // Removing Dependency
            MockRepository.Setup(d => d.GetTemplate()).Returns(new List<QuizRTTemplate>
                {
                    new QuizRTTemplate{
                        Text = "string",
                        SparQL = "string",
                        Categ = "string",
                        CategName = "string",
                        Topic = "string",
                        TopicName = "string"
                    }
                }
            );
            
            QuizRTController quizcontroller = new QuizRTController(MockRepository.Object); // Act
            var actual = quizcontroller.Get("template");

            var okObjectResult = actual as OkObjectResult;
            // Assert.NotNull(okObjectResult);

            var actualList = okObjectResult.Value as List<QuizRTTemplate>;

            Assert.NotNull(actualList); // Assert
            Console.WriteLine("actualList.Count: "+actualList.Count);
            Assert.Equal(actualList.Count , 1);
        }
    }
}
