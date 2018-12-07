using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuizRT.Models;
using Newtonsoft.Json.Linq;

namespace QuizRTapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizRTController : ControllerBase
    {
        IQuizRTRepo quizRTRepo;
        public QuizRTController(IQuizRTRepo _quizRTRepo) {
            this.quizRTRepo = _quizRTRepo;
        }
        [HttpGet("questions")]
        public async Task<IActionResult> GetAllQuestions() {
            var allQuestions = await quizRTRepo.GetAllQuestions();
            if( allQuestions.Count() > 0 )
                return new OkObjectResult(allQuestions);
            return NotFound();
        }
        [HttpGet("questions/{topicname}")]
        public async Task<IActionResult> GetQuestionsByTopic(string topicname) {
            var allQuestionsByTopic = await quizRTRepo.GetQuestionsByTopic(topicname);
            if( allQuestionsByTopic.Count() > 0 )
                return new OkObjectResult(allQuestionsByTopic);
            return NotFound();
        }
        [HttpGet("questions/{topicname}/{numberofquestions}")]
        public async Task<IActionResult> GetNNumberOfQuestionsByTopics(string topicname, int numberofquestions) {
            var nQuestionsByTopic = await quizRTRepo.GetNNumberOfQuestionsByTopics(topicname, numberofquestions);
            if(nQuestionsByTopic.Count() > 0)
                return new OkObjectResult(nQuestionsByTopic);
            return NotFound();
        }
        [HttpGet("topics")]
        public async Task<IActionResult> GetAllTopics() {
            var listOfTopics = await quizRTRepo.GetAllTopics();
            if( listOfTopics.Count() > 0)
                return new OkObjectResult(listOfTopics);
            return NotFound();
        }
        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplates(){
            //var v1 = await quizRTRepo.GetTemplate();
            List<List<Questions>> listOfTemplates = await quizRTRepo.GetTemplate();
            if( listOfTemplates.Count() > 0 )
            {
                Console.WriteLine(listOfTemplates.Count+"{}}-------------------");
               // Console.WriteLine(v1);
                Console.WriteLine(listOfTemplates[0][0]+"first value-------------------");
                return new OkObjectResult(listOfTemplates);
            }    
            return NotFound();
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> PostQuestionsAndOptoins([FromBody] QuestionGeneration qG){
            Console.WriteLine("cameeeee");
            bool statusOfQuestionPost = await quizRTRepo.GenerateQuestionsAndOptions(qG);
            if( statusOfQuestionPost )
                return new NoContentResult();
            return BadRequest();
        }
        [HttpPost("{id}")]
        public IActionResult Post(int id){
           return BadRequest();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] QuestionGeneration q){
            return BadRequest();
        }

        // DELETE api/values/
        [HttpDelete]
        public async Task<IActionResult> DeleteAllQuestions() {
            bool deleteStatus = await quizRTRepo.DeleteAllQuestions();
            if( deleteStatus )
                return new NoContentResult();
            return NotFound();
        }
        [HttpDelete("{topicname}")]
        public async Task<IActionResult> DeleteQuestionsByTopicName(string topicname) {
            bool deleteStatus = await quizRTRepo.DeleteQuestionsByTopic(topicname);
            if( deleteStatus )
                return new NoContentResult();
            return NotFound();
        }
    }
}
