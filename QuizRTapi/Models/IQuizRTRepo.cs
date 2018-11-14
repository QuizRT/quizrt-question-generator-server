using System.Collections.Generic;
using System;
namespace QuizRT.Models{
    public interface IQuizRTRepo {
        List<QuizRTTemplate> GetTemplate();
        List<Questions> GetQuestion();
        List<Options> GetOption();
        bool PostQuery(QuizRTTemplate q);
        // bool PostTemplate(Questions q);
        void DeleteTemplate();
        List<Questions> GetQuestionOnlyWithoutInsertion(QuizRTTemplate q);
    }
}