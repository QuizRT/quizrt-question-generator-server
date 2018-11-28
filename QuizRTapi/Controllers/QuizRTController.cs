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
        public QuizRTController(IQuizRTRepo _quizRTRepo){
            this.quizRTRepo = _quizRTRepo;
        }
        [HttpGet("questions")]
        public async Task<IActionResult> GetAllQuestions() {
            var qGAQ = await quizRTRepo.GetAllQuestions();
            if(qGAQ.Count() > 0)
                return new OkObjectResult(qGAQ);
            return NotFound("Database Empty!!");
        }
        [HttpGet("questions/{topicname}")]
        public async Task<IActionResult> GetQuestionsByTopic(string topicname){
            var qGQBT = await quizRTRepo.GetQuestionsByTopic(topicname);
            if(qGQBT.Count() > 0)
                return new OkObjectResult(qGQBT);
            return NotFound("Questions For Topic "+topicname+" Not Found");
        }
        [HttpGet("topics")]
        public IActionResult GetAllTopics(){
            var lT = quizRTRepo.GetAllTopics();
            if( lT.Count() > 0)
                return new OkObjectResult(lT);
            return NotFound("No Topics!");
        }
        [HttpGet("templates")]
        public IActionResult GetTemplate(){
            var lT = quizRTRepo.GetTemplate();
            if( lT.Count() > 0 )
                return new OkObjectResult(lT);
            return NotFound("No Templates!");
        }
        // GET api/quizrt
        // [HttpGet("{variable}")]
        // public IActionResult Get(string variable){
        //     if ( variable == "template" ) {
        //         List<QuizRTTemplate> Lqt = quizRTRepo.GetTemplate();
        //         if( Lqt.Count > 0 )
        //             return Ok(Lqt);
        //     } else if ( variable == "question" ) {
        //         List<Questions> Lq = quizRTRepo.GetQuestion();
        //         if( Lq.Count > 0 )
        //             return Ok(Lq);
        //     } else if ( variable == "option" ) {
        //         List<Options> Lo = quizRTRepo.GetOption();
        //         if( Lo.Count > 0 )
        //             return Ok(Lo);
        //     }
        //     return NotFound("Empty Database {TABLE: "+variable+"}");
        // }

        // POST api/values
        [HttpPost]
        // public async Task<IActionResult> Post([FromBody] QuestionGeneration qG){
        public IActionResult Post([FromBody] QuestionGeneration qG){
            Task<bool> dataReturns = System.Threading.Tasks.Task<string>.Run(() => quizRTRepo.PostQuestionGeneration(qG).Result);
            bool qPQG = dataReturns.Result;
            // bool qPQG = await quizRTRepo.PostQuestionGeneration(qG);
            if( qPQG ){
                return new OkObjectResult("Success!!");
            }
            return BadRequest("Database Error!! {POST}");
        }
        [HttpPost("{id}")]
        public IActionResult Post(int id, [FromBody] QuestionGeneration q){
        //    List<Questions> all_data =  quizRTRepo.GetQuestionOnlyWithoutInsertion(q);
           return Ok("all_data");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] QuestionGeneration q){
        }

        // DELETE api/values/5
        // [HttpDelete("{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(){
            bool deleteStatus = await quizRTRepo.DeleteAllQuestionGenerationItems();
            if( deleteStatus )
                return new OkObjectResult("Deletion Successful");
            return BadRequest("Something Went Wrong!! No Items");
        }
        [HttpDelete("{topicname}")]
        public async Task<IActionResult> Delete(string topicname){
            bool deleteStatus = await quizRTRepo.DeleteQuestionGenerationItemsByTopic(topicname);
            if( deleteStatus )
                return new OkObjectResult("All Enteries Of Topic "+topicname+" Deleted Succesfully.");
            return BadRequest("Something Went Wrong!! @["+topicname+" Items Not Present.]");
        }
    }
}
