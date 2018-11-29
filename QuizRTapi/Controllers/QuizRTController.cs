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
            var qGAQ = await quizRTRepo.GetAllQuestions();
            if(qGAQ.Count() > 0)
                return new OkObjectResult(qGAQ);
            return NotFound();
        }
        [HttpGet("questions/{topicname}")]
        public async Task<IActionResult> GetQuestionsByTopic(string topicname) {
            var qGQBT = await quizRTRepo.GetQuestionsByTopic(topicname);
            if(qGQBT.Count() > 0)
                return new OkObjectResult(qGQBT);
            return NotFound();
        }
        [HttpGet("topics")]
        public IActionResult GetAllTopics() {
            var lT = quizRTRepo.GetAllTopics();
            if( lT.Count() > 0)
                return new OkObjectResult(lT);
            return NotFound();
        }
        [HttpGet("templates")]
        public IActionResult GetTemplates(){
            var lT = quizRTRepo.GetTemplate();
            if( lT.Count() > 0 )
                return new OkObjectResult(lT);
            return NotFound();
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> PostQuestionsAndOptoins([FromBody] QuestionGeneration qG){
        // public IActionResult Post([FromBody] QuestionGeneration qG){
            // Task<bool> dataReturns = System.Threading.Tasks.Task<string>.Run(() => quizRTRepo.PostQuestionGeneration(qG).Result);
            // bool qPQG = dataReturns.Result;
            bool qPQG = await quizRTRepo.PostQuestionGeneration(qG);
            if( qPQG )
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
            bool deleteStatus = await quizRTRepo.DeleteAllQuestionGenerationItems();
            if( deleteStatus )
                return new NoContentResult();
            return NotFound();
        }
        [HttpDelete("{topicname}")]
        public async Task<IActionResult> DeleteQuestionsByTopicName(string topicname) {
            bool deleteStatus = await quizRTRepo.DeleteQuestionGenerationItemsByTopic(topicname);
            if( deleteStatus )
                return new NoContentResult();
            return NotFound();
        }
    }
}
