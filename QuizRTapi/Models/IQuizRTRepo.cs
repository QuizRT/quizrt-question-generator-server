using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace QuizRT.Models{
    public interface IGameContext {
        IMongoCollection<QuestionGeneration> QuestionGenerationCollection { get; }
    }
    public interface IQuizRTRepo {
        Task<IEnumerable<QuestionGeneration>> GetAllQuestions();
        Task<IEnumerable<Questions>> GetQuestionsByTopic(string topicName);
        Task<bool> DeleteAllQuestions();
        Task<bool> DeleteQuestionsByTopic(string topicName);
        Task<bool> GenerateQuestionsAndOptions(QuestionGeneration qG);
        Task<List<string>> GetTemplate();
        Task<List<string>> GetAllTopics();
        // List<QuizRTTemplate> GetTemplate();
        // List<Questions> GetQuestion();
        // List<Options> GetOption();
        // bool PostQuery(QuizRTTemplate q);
        // bool PostQueryNew(QuizRTTemplate q);
        // // bool PostTemplate(Questions q);
        // void DeleteTemplate();
        // List<Questions> GetQuestionOnlyWithoutInsertion(QuizRTTemplate q);
    }
}