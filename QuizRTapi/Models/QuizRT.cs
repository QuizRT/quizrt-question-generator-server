using System.ComponentModel.DataAnnotations; // for using [Key], [Required] etc.
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
namespace QuizRT.Models{
    public class QuestionGeneration {
        [BsonId]
        public ObjectId Id { get; set; }
        // [BsonRepresentation(BsonType.ObjectId)]
        // public string Id { get; set; }
        public string Text { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string TopicId { get; set; }
        public string TopicName { get; set; }
        public List<Questions> QuestionsList { get; set; }
    }
    public class Questions {
        public string Question { get; set; }
        public string CorrectOption { get; set; }
        public List<OtherOptions> OtherOptionsList { get; set; }
    }
    public class OtherOptions {
        public string Option { get; set; }
    }
    // public class QuizRTTemplate{
    //     [Key]
    //     public int TempId { get; set; }
    //     public string Text { get; set; }
    //     public string SparQL { get; set; }
    //     public string Categ { get; set; }
    //     public string CategName { get; set; }
    //     public string Topic { get; set; }
    //     public string TopicName { get; set; }
    // }
    // public class Questions{
    //     [Key]
    //     public int QuestionsId { get; set; }
    //     public string Categ { get; set; }
    //     public string Topic { get; set; }
    //     public string QuestionGiven { get; set; }
    //     public List<Options> QuestionOptions { get; set; }
    // }
    // public class Options{
    //     [Key]
    //     public int OptionsId { get; set; }
    //     public string OptionGiven { get; set; }
    //     public bool IsCorrect { get; set; }
    //     public int QuestionsId { get; set; }
    // }
    public class universal_object{
        public string mainobject {get;set;}
        public string predicate {get;set;}
    }

    public class List_template_corresponding_ques {
        public string template {get;set;}
        public List<Questions> Coressponding_questions {get;set;}
        public  Questions Single_Question {get;set;}

    }
}