using System.ComponentModel.DataAnnotations; // for using [Key], [Required] etc.
using System.Collections.Generic;
namespace QuizRT.Models{
    public class QuizRTTemplate{
        [Key]
        public int TempId { get; set; }
        public string Text { get; set; }
        public string SparQL { get; set; }
        public string Categ { get; set; }
        public string CategName { get; set; }
        public string Topic { get; set; }
        public string TopicName { get; set; }
    }
    public class Questions{
        [Key]
        public int QuestionsId { get; set; }
        public string Categ { get; set; }
        public string Topic { get; set; }
        public string QuestionGiven { get; set; }
        public List<Options> QuestionOptions { get; set; }
    }
    public class Options{
        [Key]
        public int OptionsId { get; set; }
        public string OptionGiven { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionsId { get; set; }
    }
    public class universal_object{
        public string mainobject {get;set;}
        public string predicate {get;set;}
    }
}