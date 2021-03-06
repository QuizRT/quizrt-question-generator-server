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
        Task<IEnumerable<QuestionGeneration>> GetQuestionsByTopic(string topicName);
        Task<IEnumerable<Questions>> GetNNumberOfQuestionsByTopics(string topicName, int numberOfQuestions);
        Task<bool> DeleteAllQuestions();
        Task<bool> DeleteQuestionsByTopic(string topicName);
        Task<bool> GenerateQuestionsAndOptions(QuestionGeneration qG);
        Task<List<string>> GetTemplate();
        Task<List<List_template_corresponding_ques>> List_template_corresponding_ques();

        Task<List<List<Questions>>> Getfirst_question_as_sample();
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