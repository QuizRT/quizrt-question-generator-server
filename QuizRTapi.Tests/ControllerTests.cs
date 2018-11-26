using System;
using Xunit;
using Moq;
using System.Linq;
using System.Collections.Generic;
using QuizRT.Models;
using Microsoft.AspNetCore.Mvc;
using QuizRTapi.Controllers;
using System.Threading.Tasks;

namespace QuizRTapi.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task GetTemplate(){
            DummyData DD = new DummyData();
            IEnumerable<QuestionGeneration> dummy = DD.DummyMock();  // Arrange
            
            Mock<IQuizRTRepo> MockRepository = new Mock<IQuizRTRepo>(); // Removing Dependency
            MockRepository.Setup<Task<IEnumerable<QuestionGeneration>>>
                (d => d.GetAllQuestions())
                    .Returns(Task.FromResult<IEnumerable<QuestionGeneration>>(dummy));
            
            QuizRTController quizcontroller = new QuizRTController(MockRepository.Object); // Act
            var actual = await quizcontroller.Get();

            var okObjectResult = actual as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var actualList = okObjectResult.Value as IEnumerable<QuestionGeneration>;

            Assert.NotNull(actualList); // Assert
            // Console.WriteLine("actualList.Count: "+actualList.Count);
            // Assert.Equal(actualList , 1);
        }
    }
}
