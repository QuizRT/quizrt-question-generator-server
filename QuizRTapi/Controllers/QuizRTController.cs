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
        // GET api/values
        [HttpGet("{variable}")]
        public IActionResult Get(string variable){
            if ( variable == "template" ) {
                List<QuizRTTemplate> Lqt = quizRTRepo.GetTemplate();
                if( Lqt.Count > 0 )
                    return Ok(Lqt);
            } else if ( variable == "question" ) {
                List<Questions> Lq = quizRTRepo.GetQuestion();
                if( Lq.Count > 0 )
                    return Ok(Lq);
            } else if ( variable == "option" ) {
                List<Options> Lo = quizRTRepo.GetOption();
                if( Lo.Count > 0 )
                    return Ok(Lo);
            }
            return NotFound("Empty Database {TABLE: "+variable+"}");
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] QuizRTTemplate q){
            Console.WriteLine(q);
            if( quizRTRepo.PostQuery(q) ){
                return Created("/api/quizrt",q);
            }
            return BadRequest("Database Error!! {POST}");
        }
        [HttpPost("{id}")]
        public IActionResult Post(int id, [FromBody] QuizRTTemplate q){
           List<Questions> all_data =  quizRTRepo.GetQuestionOnlyWithoutInsertion(q);
           return Ok(all_data);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Questions q){
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id){
            quizRTRepo.DeleteTemplate();
        }
    }
}
