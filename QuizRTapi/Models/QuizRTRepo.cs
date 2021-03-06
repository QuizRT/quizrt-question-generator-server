using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using MongoDB.Bson;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;
using TopicEngine.Services;

// using TopicEngine.Services;

namespace QuizRT.Models{
    public class QuizRTRepo : IQuizRTRepo {
        int NumberOfQuestions = 10000;
        static Random random = new Random();
        QuizRTContext context;
        public QuizRTRepo(QuizRTContext _context) {
            this.context = _context;
        }

        public async Task<IEnumerable<QuestionGeneration>> GetAllQuestions() {
            return await context.QuestionGenerationCollection.Find(_  => true).ToListAsync();
        }
        public async Task<IEnumerable<QuestionGeneration>> GetQuestionsByTopic(string topicName) {
            FilterDefinition<QuestionGeneration> filter = Builders<QuestionGeneration>
                                                            .Filter.Eq(m => m.TopicName, topicName);
            var questionCursor = await context.QuestionGenerationCollection.FindAsync(filter);
            return await questionCursor.ToListAsync();
        }
        public async Task<IEnumerable<Questions>> GetNNumberOfQuestionsByTopics(string topicName, int numberOfQuestions){
            FilterDefinition<QuestionGeneration> filter = Builders<QuestionGeneration>
                                                            .Filter.Eq(n => n.TopicName, topicName);
            var questionCursor = await context.QuestionGenerationCollection.FindAsync(filter);
            return RandomSetOfQuestionsByTopic(await questionCursor.ToListAsync(), numberOfQuestions);
        }
        public List<Questions> RandomSetOfQuestionsByTopic(List<QuestionGeneration> allQuestionsGenerationByTopic, int numberOfQuestions) {
            List<Questions> setOfRandomQuestionByTopic = new List<Questions>();
            if (allQuestionsGenerationByTopic.Count > 0)
            {
                List<Questions> allQuestionsByTopic = new List<Questions>();
                allQuestionsGenerationByTopic.ForEach(a => {
                    if(a.QuestionsList != null){
                        a.QuestionsList.ForEach(b => {
                            allQuestionsByTopic.Add(b);
                        });
                    }
                });
                List<int> uniqueNoList = UniqueRandomNumberList(allQuestionsByTopic.Count, numberOfQuestions);
                uniqueNoList.ForEach(a => {
                    setOfRandomQuestionByTopic.Add(allQuestionsByTopic[a]);
                });
            }
            return setOfRandomQuestionByTopic;
        }
        public static List<int> UniqueRandomNumberList(int maxRange, int totalRandomnoCount)    
        {   
            List<int> noList = new List<int>();    
            int count = 0;    
            Random r = new Random();    
            List<int> listRange = new List<int>();    
            for (int i = 0; i < totalRandomnoCount; i++)    
            {    
                listRange.Add(i);    
            }    
            while (listRange.Count > 0)    
            {    
                int item = r.Next(maxRange);// listRange[];    
                if (!noList.Contains(item) && listRange.Count > 0)    
                {    
                    noList.Add(item);    
                    listRange.Remove(count);    
                    count++;    
                }    
            }    
            return noList;    
        }
        public async Task<List<string>> GetAllTopics() {
            FilterDefinition<QuestionGeneration> filter = Builders<QuestionGeneration>.Filter.Empty;
            var topicCursor = await context.QuestionGenerationCollection.DistinctAsync<string>("TopicName", filter);
            return await topicCursor.ToListAsync();
        }

        public async Task<List<string>> GetTemplate() {
            var templateCursor = context.QuestionGenerationCollection.Find(_ => true).Project(u => u.Text);
            // var template = context.QuestionGenerationCollection.Find(_ => true).Project(u =>u.Text && u.Text);
            //      var zooWithAnimalFilter = Builders<Zoo>.Filter
            // .ElemMatch(z => z.Animals, a => a.Name == animalName);
            return await templateCursor.ToListAsync();
        }

        public async Task<List<List_template_corresponding_ques>> List_template_corresponding_ques()
        {
            List<List_template_corresponding_ques> temp = new List<List_template_corresponding_ques>();
            
            List<List<Questions>> t1 = await Getfirst_question_as_sample();
            List<string> t2 = await GetTemplate();

            for(int i=0;i< t2.Count;i++)
            {
                if(t1[i]!=null)
                {
                List_template_corresponding_ques dummy = new List_template_corresponding_ques();
                if(t1[i].Count < 10)
                    dummy.Coressponding_questions = t1[i].GetRange(0,t1[i].Count);
                else
                    dummy.Coressponding_questions = t1[i].GetRange(0,10);
                dummy.template = t2[i];
                temp.Add(dummy);
                }
            }

            return temp;
        }



        public async Task<List<List<Questions>>> Getfirst_question_as_sample() {
            var templateCursor = context.QuestionGenerationCollection.Find(_ => true).Project(u => u.QuestionsList);
            // var template = context.QuestionGenerationCollection.Find(_ => true).Project(u =>u.Text && u.Text);
        //      var zooWithAnimalFilter = Builders<Zoo>.Filter
        // .ElemMatch(z => z.Animals, a => a.Name == animalName);
            Console.WriteLine(templateCursor+"000000000000");
            return await templateCursor.ToListAsync();
        }


        public async Task<bool> DeleteAllQuestions() {
            List<string> all_topics = await GetAllTopics();
            DeleteResult deleteResult = await context.QuestionGenerationCollection.DeleteManyAsync(_ => true);
            if ( deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0 ){
                Console.WriteLine(deleteResult.DeletedCount+" Items Deleted.");
                TopicBroadcaster topicBroadcaster_all_deleted  = new TopicBroadcaster();
                topicBroadcaster_all_deleted.Publish_All_Topics_as_deleted(all_topics);
                Console.WriteLine("All the topics are deleted --------------");
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteQuestionsByTopic(string topicName) {
            FilterDefinition<QuestionGeneration> filter = Builders<QuestionGeneration>
                                                            .Filter.Eq(m => m.TopicName, topicName);
            DeleteResult deleteResult = await context.QuestionGenerationCollection.DeleteManyAsync(filter);
            if( deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0 ){
                Console.WriteLine(deleteResult.DeletedCount+" Items Deleted.");
                TopicBroadcaster topicBroadcaster_deleted_single  = new TopicBroadcaster();
                 topicBroadcaster_deleted_single.PublishTopic_as_deleted(topicName);
                 Console.WriteLine("----------------"+topicName+" deleted --------------");
                return true;
            }
            return false;
        }


        public async Task<QuestionGeneration> Do_Regenerate_Template(string text) {
            //var templateCursor = context.QuestionGenerationCollection.Find(_ => true).Project(u => u.Text);
            List<QuestionGeneration> Selected_object = new List<QuestionGeneration>();
            QuestionGeneration Dublicate_copy = new QuestionGeneration();

            FilterDefinition<QuestionGeneration> filter = Builders<QuestionGeneration>
                                                          .Filter.Eq(m => m.Text, text);
            var questionCursor = await context.QuestionGenerationCollection.FindAsync(filter);
            Selected_object = await questionCursor.ToListAsync();

            Dublicate_copy.CategoryName =  Selected_object[0].CategoryName;
            Dublicate_copy.Text =  Selected_object[0].Text;
            Dublicate_copy.TopicName =  Selected_object[0].TopicName;
            Dublicate_copy.CategoryId =  Selected_object[0].CategoryId;
            Dublicate_copy.TopicId =  Selected_object[0].TopicId;
            Console.WriteLine("assigned to another object-----------");
            DeleteResult deleteResult = await context.QuestionGenerationCollection.DeleteOneAsync(filter);
            if( deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0 ){
                //Console.WriteLine("deleted the template from the database======");
                Console.WriteLine(deleteResult.DeletedCount+" Items Deleted.");
                return Dublicate_copy;
            }
            return null;
        }
        public async Task<bool> GenerateQuestionsAndOptions(QuestionGeneration qT) {
            if(qT.CategoryName=="" && qT.TopicName=="")
            {
                Console.WriteLine("Entered for the renewal---------");
                QuestionGeneration delete_result = await Do_Regenerate_Template(qT.Text);
                if(delete_result!=null)
                 qT = delete_result;
                else
                 return false;
            }

            Console.WriteLine("Finished  Renewal  Check");
            FilterDefinition<QuestionGeneration> filter = Builders<QuestionGeneration>
                                                            .Filter.Eq(m => m.Text, qT.Text);
            var checkForTemplate = await context.QuestionGenerationCollection.Find(filter).FirstOrDefaultAsync();
            if( checkForTemplate == null )
            {
                List<string> getPresentTopics = await GetAllTopics();
                await context.QuestionGenerationCollection.InsertOneAsync(qT);
                var insertedTemplate = await context.QuestionGenerationCollection.Find(filter).FirstOrDefaultAsync();
                ObjectId currentTemplateId = insertedTemplate.Id;
                bool check = await InsertQuestionsAndOptions(qT,currentTemplateId);
                if(check)
                {
                    // getPresentTopics.ForEach(i => Console.Write("-- {0}\t", i));
                    if (!getPresentTopics.Contains(qT.TopicName, StringComparer.OrdinalIgnoreCase))
                    {
                        PublishTopic(qT.TopicName);
                    }
                    return true;
                }
            }
            return false;
        }
        public void PublishTopic(string newTopicAdded)
        {
            TopicBroadcaster topicBroadcasterProvider = new TopicBroadcaster();
            topicBroadcasterProvider.BroadcastTopics(newTopicAdded);
        }
        public async Task<bool> InsertQuestionsAndOptions(QuestionGeneration q, ObjectId currentTemplateId) {
            var otherOptionsList = GenerateOtherOptions(q.CategoryName);

            string sparQL = "SELECT ?cidLabel ?authortitleLabel WHERE {?cid wdt:P31 wd:"+q.TopicId+".?cid wdt:"+q.CategoryId+" ?authortitle .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}LIMIT "+NumberOfQuestions+"";
            // string sparQL2 = $@"SELECT ?personLabel WHERE {{ ?person wdt:{q.Topic} wd:{q.Categ} . SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . } }LIMIT "+NumberOfQuestions+""; // Nishant
            Task<List<universal_object>> subjectForQuestion = System.Threading.Tasks.Task<string>.Run(() => GetSubjectsForQuestion(sparQL).Result);
            List<universal_object> listOfSubjectForQuestion = subjectForQuestion.Result;
            
            List<Questions> questionsList = new List<Questions>(); 
            string replacementStrSubject = '['+GetBetween(q.Text,"[","]")+']';
            string replacementStrObject = '('+GetBetween(q.Text,"(",")")+')';
            for(int i=0; i<listOfSubjectForQuestion.Count; i++)
            {
                if(!(listOfSubjectForQuestion[i].mainobject[0] == 'Q' && IsDigitsOnly(listOfSubjectForQuestion[i].mainobject.Substring(1)))) {
                    string questionText = q.Text;
                    questionText = questionText.Replace(replacementStrObject, q.CategoryName);
                    questionText = questionText.Replace(replacementStrSubject, listOfSubjectForQuestion[i].mainobject);

                    List<OtherOptions> listOfOtherOptions = new List<OtherOptions>();
                    // int iteratorForListOfOptions = 0;
                    int increaser = 0;
                    // otherOptionsList = otherOptionsList.GetRange(1, otherOptionsList.Count).Append(otherOptionsList.First()).ToList(); 

                    for(int j=0; j<3+increaser; j++)
                    {
                        if(otherOptionsList[j] != "" && !(otherOptionsList[j][0] == 'Q' && IsDigitsOnly(otherOptionsList[j].Substring(1))))
                        {
                            OtherOptions otherOptionObject = new OtherOptions();
                            otherOptionObject.Option = otherOptionsList[j];
                            listOfOtherOptions.Add(otherOptionObject);
                            // if(iteratorForListOfOptions+3 < otherOptionsList.Count)
                            //     iteratorForListOfOptions++;
                            // else
                            //     iteratorForListOfOptions = 0;
                        } 
                        else
                        {
                            increaser++;
                        }
                    }
                    otherOptionsList = Shift(otherOptionsList);

                    Questions questionObject = new Questions();
                    questionObject.Question = questionText;
                    questionObject.CorrectOption = listOfSubjectForQuestion[i].predicate;
                    questionObject.OtherOptionsList = listOfOtherOptions;

                    questionsList.Add(questionObject);
                }
            }
            QuestionGeneration questionGeneratedObject = new QuestionGeneration();
            questionGeneratedObject.Id = currentTemplateId;
            questionGeneratedObject.Text = q.Text;
            questionGeneratedObject.CategoryId = q.CategoryId;
            questionGeneratedObject.CategoryName = q.CategoryName;
            questionGeneratedObject.TopicId = q.TopicId;
            questionGeneratedObject.TopicName = q.TopicName;
            questionGeneratedObject.QuestionsList = questionsList;
            ReplaceOneResult updateResult = await context.QuestionGenerationCollection.
                        ReplaceOneAsync(filter: g => g.Id == questionGeneratedObject.Id, replacement: questionGeneratedObject);
            
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
        static List<T> Shift<T>(List<T> list)
        {
            return list.GetRange(1, list.Count - 1).Append(list.First()).ToList();
        }
        bool IsDigitsOnly(string str) {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }
        async Task<List<universal_object>> GetSubjectsForQuestion(string sparQL) { // Generating question related to some object other than occupation
            List<universal_object> universal_list_objects = new List<universal_object>();
            string baseUrl = "https://query.wikidata.org/sparql?query="+sparQL+"&format=json";
            //The 'using' will help to prevent memory leaks.
            //Create a new instance of HttpClient
            using (HttpClient client = new HttpClient())
            //Setting up the response...         
            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            using (HttpContent content = res.Content){
                string data = await content.ReadAsStringAsync();
                // JObject data = await content.ReadAsAsync<JObject>();
                //Console.WriteLine(data);
                JObject json = JObject.Parse(data);
                JArray j  = ((JArray)json["results"]["bindings"]);
                if (data != null)
                {
                    for(int i=0; i < j.Count ; i++)
                    { 
                        universal_object s_universe = new universal_object(); 
                        s_universe.mainobject = (j[i]["cidLabel"]["value"].ToString());
                        s_universe.predicate = (string)j[i]["authortitleLabel"]["value"];
                        universal_list_objects.Add(s_universe);
                    }
                    
                    return universal_list_objects;
                }
                return new List<universal_object>();
            }
        }

        public static string GetBetween(string strSource, string strStart, string strEnd) {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else 
            {
                return "";
            }
        }
        public List<string> GenerateOtherOptions(string CategName) {

            Task<List<string>> optionIdReturned = System.Threading.Tasks.Task<string>.Run(() => GetOptionId(CategName).Result);
            List<string> optionIDList = optionIdReturned.Result;
            List<string> listOfOptions = new List<string>();  
            for(int i=0; i<optionIDList.Count; i++)
            {
                string sparQL = "SELECT ?cidLabel WHERE {?cid wdt:P31 wd:"+optionIDList[i]+" .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }} LIMIT "+NumberOfQuestions+"";
                Task<List<string>> otherOptionsReturned = System.Threading.Tasks.Task<string>.Run(() => GetOtherOptions(sparQL).Result);
                List<string> otherOptionsList = otherOptionsReturned.Result;
                listOfOptions.AddRange(otherOptionsList);
            }
            return listOfOptions;
        }
        async Task<List<string>> GetOptionId(string subject) { // Generating question related to some object other than occupation

            string baseUrl = "https://www.wikidata.org/w/api.php?origin=*&action=wbsearchentities&search="+subject+"&language=en&format=json";
            //The 'using' will help to prevent memory leaks.
            //Create a new instance of HttpClient
            using (HttpClient client = new HttpClient())
            //Setting up the response...         
            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            using (HttpContent content = res.Content){
                string data = await content.ReadAsStringAsync();
                JObject json = JObject.Parse(data);
                JArray j  = ((JArray)json["search"]);

                List<string> listOfOptionId = new List<string>();
                if (data != null)
                {
                    for(int i=0; i < j.Count ; i++)
                    {
                        if(j[i]["id"].ToString().IndexOf('Q') > -1)
                        {
                            listOfOptionId.Add(j[i]["id"].ToString());
                        }
                    }
                }
                return listOfOptionId;
            }
        }
        async Task<List<string>> GetOtherOptions(string sparQL) { // Generating question related to some object other than occupation

            string baseUrl = "https://query.wikidata.org/sparql?query="+sparQL+"&format=json";
            //The 'using' will help to prevent memory leaks.
            //Create a new instance of HttpClient
            using (HttpClient client = new HttpClient())
            //Setting up the response...         
            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            using (HttpContent content = res.Content){
                string data = await content.ReadAsStringAsync();
                JObject json = JObject.Parse(data);
                JArray j  = ((JArray)json["results"]["bindings"]);
                List<string> otherOps = new List<string>();
                if (data != null)
                {
                    for(int i=0; i < j.Count ; i++)
                    {
                        otherOps.Add(j[i]["cidLabel"]["value"].ToString());
                    }
                }
                return otherOps;
            }
        }

// ---------------------------------
        // public List<QuizRTTemplate> GetTemplate(){
        //     return context.QuizRTTemplateT.ToList();
        // }
        // public List<Questions> GetQuestion(){
        //     return context.QuestionsT
        //                     .Include( n => n.QuestionOptions )
        //                     .ToList();
        // }
        // public List<Options> GetOption(){
        //     return context.OptionsT.ToList();
        // }

        // ------------------------------------------------

//         public List<Questions> GetQuestionOnlyWithoutInsertion(QuizRTTemplate template){
//             List<Questions> g_question_notable = new List<Questions>();
//             Task<string> id= Gettopic_id("https://www.wikidata.org/w/api.php?action=wbsearchentities&search="+template.TopicName+"&language=en&format=json");
//             string f_id = id.Result;
//             string sparQL = "SELECT ?personLabel WHERE { ?person wdt:P106 wd:"+f_id+" . SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}Limit 100";
//             Task<List<string>> questions = GetQuestionData(sparQL);
//             List<string> all_questions_without_tables = questions.Result;
//             Task<List<string>> options= GetOptionData("SELECT ?cid ?options WHERE {?cid wdt:P31 wd:Q28640. OPTIONAL {?cid rdfs:label ?options filter (lang(?options) = 'en') . }}Limit 100");
//             List<string> all_options = options.Result;
//             for(int i=0;i<all_questions_without_tables.Count;i++)
//             {
//                 Questions s_object = new Questions();
//                 s_object.QuestionGiven="What is "+all_questions_without_tables[i]+" Occupation ?";
//                 List<Options> mut_options_single_q = randomizeOptions(all_options,template.TopicName);
//                 s_object.QuestionOptions=mut_options_single_q;
//                 g_question_notable.Add(s_object);
//             }
//             return g_question_notable;
//         }

//        public List<Options> randomizeOptions(List<string> optionReviewList,string topic_name){
//            List<string> other_options = new List<string>();
//            List<Options> g_List_options_notable = new List<Options>();
//            List<int> randomNumber = getRandonNumber(0, optionReviewList.Count-1, optionNumber-1);
//            for(int i=0; i < randomNumber.Count ; )
//            {
//                Options g_options_notable_f = new Options();
//                g_options_notable_f.OptionGiven=optionReviewList[randomNumber[i]];
//                g_options_notable_f.IsCorrect=false;
//                g_List_options_notable.Add(g_options_notable_f); 
//                i++;
//            }
//            Options g_options_notable = new Options();
//                g_options_notable.OptionGiven=topic_name;
//                g_options_notable.IsCorrect=true;
//                g_List_options_notable.Add(g_options_notable);  
//            return g_List_options_notable;
//        }

//        async Task<string> Gettopic_id(string sparQL){
//            //string baseUrl = "https://query.wikidata.org/sparql?query="+sparQL+"&format=json";
//            //The 'using' will help to prevent memory leaks.
//            //Create a new instance of HttpClient
//            using (HttpClient client = new HttpClient())
//            //Setting up the response...
//            using (HttpResponseMessage res = await client.GetAsync(sparQL))
//            using (HttpContent content = res.Content){
//                string data = await content.ReadAsStringAsync();
//                // JObject data = await content.ReadAsAsync<JObject>();
//                JObject json = JObject.Parse(data);
//                JArray J = (JArray)json["search"];
//                string str = (string)J[0]["id"];
//                return str;
//            }
//        }
// // ------------------------------------------------
//         public bool PostQuery(QuizRTTemplate qT){    Console.WriteLine("---Inside-PostQuery---");
//             // JObject jo = (JObject)(q);
//             // string categId = jo["categ"].ToString();
//             // string topicId = jo["topic"].ToString();
//             // string categName = jo["categName"].ToString();
//             // string topicName = jo["topicName"].ToString();
//             if( context.QuizRTTemplateT.FirstOrDefault( n => n.Categ == qT.Categ) == null ){
//                 // QuizRTTemplate qT = new QuizRTTemplate();
//                 // qT.Categ = categId;
//                 // qT.CategName = categName;
//                 // qT.Topic = topicId;
//                 // qT.TopicName = topicName;
//                 context.QuizRTTemplateT.Add(qT);
//                 context.SaveChanges();  Console.WriteLine("---Template-Table-Inserted---");

//                 if( context.QuestionsT.FirstOrDefault( n => n.Categ == qT.Categ) == null ){
//                     if(qT.TopicName == "Occupation"){   Console.WriteLine("---Inside-Occupation---");
//                         if ( GenerateQuestion(qT) && GenerateOptions(qT) ) 
//                             return true;
//                     } else {
//                         if( GenerateQuestion(qT) )
//                             return true;
//                     }
//                 }
//                 return true;
//             }
//             return false;
//         }
//         // public bool PostTemplate(Questions q){// QuizRTTemplate
//         //     // if( context.QuizRTTemplateT.FirstOrDefault( n => n.Categ == q.Categ) == null ){
//         //     //     context.QuizRTTemplateT.Add(q);
//         //     //     context.SaveChanges();
//         //     //     return true;
//         //     // }
//         //     // return false;
//         //     context.QuestionsT.Add(q);
//         //     context.SaveChanges();
//         //     return true;
//         // }
//         public void DeleteAllQuestionGenerationItems(){
//             List<QuizRTTemplate> Lqt = context.QuizRTTemplateT.ToList();
//             if( Lqt.Count > 0 ){
//                 context.Database.ExecuteSqlCommand("TRUNCATE TABLE QuizRTTemplateT");
//                 // context.RemoveRange(Lqt);
//                 // context.SaveChanges();
//             }
//             List<Questions> LqtQ = context.QuestionsT.ToList();
//             if( LqtQ.Count > 0 ){
//                 // context.Database.ExecuteSqlCommand("TRUNCATE TABLE [QuestionsT]");
//                 context.RemoveRange(LqtQ);
//                 context.SaveChanges();
//             }
//             List<Options> Lops = context.OptionsT.ToList();
//             if( Lops.Count > 0 ){
//                 // context.Database.ExecuteSqlCommand("TRUNCATE TABLE [OptionsT]");
//                 context.RemoveRange(Lops);
//                 context.SaveChanges();
//             }
//         }

// // ------------------------------------------

//         public bool GenerateQuestion(QuizRTTemplate q) {    Console.WriteLine("---Inside-GenerateQuestion---");
//             if(q.TopicName == "Occupation"){
//                 string sparQL = "SELECT ?personLabel WHERE { ?person wdt:"+q.Topic+" wd:"+q.Categ+" . SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . } }LIMIT "+NumberOfQuestions+"";
//                 // string sparQL2 = $@"SELECT ?personLabel WHERE {{ ?person wdt:{q.Topic} wd:{q.Categ} . SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . } }LIMIT "+NumberOfQuestions+""; // Nishant
//                 Task<List<string>> dataReturns = System.Threading.Tasks.Task<string>.Run(() => GetQuestionData(sparQL).Result);
//                 List<string> quesReviewList = dataReturns.Result;

//                 List<Questions> qL = new List<Questions>();
//                 for(int i=0; i<quesReviewList.Count; i++){
//                     Questions ques = new Questions();
//                     ques.QuestionGiven = "What is "+quesReviewList[i]+" "+q.TopicName+"?";
//                     ques.Topic = q.TopicName;
//                     ques.Categ = q.CategName;
//                     qL.Add(ques);
//                 }
//                 context.QuestionsT.AddRange(qL);
//                 context.SaveChanges();
//                 return true;
//             }
//             else if( q.TopicName != "Occupation" ){
//                 string sparQL = "";
//                 //List<string> other_options = new List<string>();
//                 Task<string> id= Gettopic_id("https://www.wikidata.org/w/api.php?action=wbsearchentities&search="+q.TopicName+"&language=en&format=json");
//                 string f_id = id.Result;
//                 if(q.TopicName=="Book")
//                     sparQL = "SELECT ?cidLabel ?authortitleLabel WHERE {?cid wdt:P31 wd:"+f_id+".?cid wdt:P50 ?authortitle .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}Limit 10";

//                 else if(q.TopicName=="princely state of the British Raj"){
//                     sparQL = "SELECT ?cidLabel ?authortitleLabel WHERE {?cid wdt:P31 wd:Q1336152 . ?cid wdt:P17 ?authortitle .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}Limit 10";
//                 }
//                 else if(q.TopicName=="state of the United States")
//                     sparQL = "SELECT ?cidLabel ?authortitleLabel WHERE {?cid wdt:P31 wd:"+q.Topic+".?cid wdt:P138 ?authortitle .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}Limit 10";

//                 else if(q.TopicName=="business")
//                     sparQL = "SELECT ?cidLabel ?authortitleLabel WHERE {?cid wdt:P31 wd:"+q.Topic+".?cid wdt:P571 ?authortitle .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}Limit 10";

//                 else if(q.TopicName!="princely state of the British Raj")
//                     sparQL = "SELECT ?cidLabel ?authortitleLabel WHERE {?cid wdt:P31 wd:"+q.Topic+".?cid wdt:P17 ?authortitle .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}Limit 10";

//                 else if(q.TopicName!="princely state of the British Raj")
//                     sparQL = "SELECT ?cidLabel ?authortitleLabel WHERE {?cid wdt:P31 wd:"+q.Topic+".?cid wdt:P17 ?authortitle .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}Limit 10";

//                 else if(q.TopicName!="princely state of the British Raj")
//                     sparQL = "SELECT ?cidLabel ?authortitleLabel WHERE {?cid wdt:P31 wd:"+q.Topic+".?cid wdt:P17 ?authortitle .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}Limit 10";

//                 else if(q.TopicName!="princely state of the British Raj")
//                     sparQL = "SELECT ?cidLabel ?authortitleLabel WHERE {?cid wdt:P31 wd:"+q.Topic+".?cid wdt:P17 ?authortitle .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}Limit 10";

//                 else if(q.TopicName!="princely state of the British Raj")
//                     sparQL = "SELECT ?cidLabel ?authortitleLabel WHERE {?cid wdt:P31 wd:"+q.Topic+".?cid wdt:P17 ?authortitle .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}Limit 10";

//                 Task<List<universal_object>> dataReturns = System.Threading.Tasks.Task<string>.Run(() => GetQuestionData_others(sparQL).Result);
//                 List<universal_object> quesReviewList = dataReturns.Result;
//                 // Console.WriteLine(quesReviewList.Count);
//                 List<Questions> qL = new List<Questions>();
                
//                 List<string> books_etc_options = GenerateOptions1(q);
                
//                 for(int i=0; i<quesReviewList.Count; i++){
//                     Questions ques = new Questions();
//                     if(q.TopicName=="Book")
//                         ques.QuestionGiven = "Who is the author of "+quesReviewList[i].mainobject+"?";
//                     else if(q.TopicName=="princely state of the British Raj"){
//                         ques.QuestionGiven = " "+quesReviewList[i].mainobject+" belongs to which country ?";
//                     }
//                     else if(q.TopicName=="state of the United States")
//                         ques.QuestionGiven = " "+quesReviewList[i].mainobject+" is named after ?";
//                     else if(q.TopicName=="business"){
//                         ques.QuestionGiven = "When was "+quesReviewList[i].mainobject+" established ?";
//                     }
//                     ques.Topic = q.TopicName;
//                     ques.Categ = q.CategName;
//                     //qL.Add(ques);
//                     context.QuestionsT.Add(ques);
//                     Options op = new Options();
//                     op.IsCorrect=true;
//                     op.QuestionsId=ques.QuestionsId;
//                     op.OptionGiven=quesReviewList[i].predicate; 
//                     context.OptionsT.Add(op);
//                     List<int> randomNumber = getRandonNumber(0, books_etc_options.Count-1, optionNumber+2);
//                     for(int j=0;j<3;j++){
//                         Options op1 = new Options();
//                         op1.IsCorrect=false;
//                         op1.QuestionsId=ques.QuestionsId;
//                         // if(books_etc_options[randomNumber[k]]!=op.OptionGiven)
//                         // {
//                         op1.OptionGiven = books_etc_options[randomNumber[j]];
//                         // j++;
//                         context.OptionsT.Add(op1);
//                         //}
//                         // k++;
//                     }  
//                 }
//                 //context.QuestionsT.AddRange(qL);
//                 context.SaveChanges();
//                 return true;
//             }
//             return false;
//         }
//         public List<string> GenerateOptions1(QuizRTTemplate q) { // For generating options other than Occupation
//             string sparQL = "";
//             if(q.TopicName=="Book")
//                 sparQL = "SELECT ?cid ?options WHERE {?cid wdt:P106 wd:Q482980. OPTIONAL {?cid rdfs:label ?options filter (lang(?options) = 'en') . }}Limit "+NumberOfQuestions*10+"";
//             else if(q.TopicName=="princely state of the British Raj")
//                 sparQL = "SELECT ?cid ?options WHERE {?cid wdt:P31 wd:Q6256. OPTIONAL {?cid rdfs:label ?options filter (lang(?options) = 'en') . }}Limit "+NumberOfQuestions*10+"";
//             else if(q.TopicName=="state of the United States")  
//                 sparQL = "SELECT ?cid ?options WHERE {?cid wdt:P166/wdt:P31 wd:Q7191. OPTIONAL {?cid rdfs:label ?options filter (lang(?options) = 'en') . }}Limit "+NumberOfQuestions*10+"";
//             else if(q.TopicName=="business")  
//                 sparQL = "SELECT DISTINCT ?person ?personLabel ?options WHERE {?person wdt:P31 wd:Q3918.?person wdt:P571 ?options SERVICE wikibase:label {bd:serviceParam wikibase:language '[AUTO_LANGUAGE],en' .}}Limit "+NumberOfQuestions*10+"";
//             Task<List<string>> dataReturns = System.Threading.Tasks.Task<string>.Run(() => GetOptionData(sparQL).Result);
//             List<string> optionReviewList = dataReturns.Result;
//             return optionReviewList;
//         }
//         async Task<List<universal_object>> GetQuestionData_others(string sparQL){ // Generating question related to some object other than occupation
//             List<universal_object> universal_list_objects = new List<universal_object>();
//             string baseUrl = "https://query.wikidata.org/sparql?query="+sparQL+"&format=json";
//             //The 'using' will help to prevent memory leaks.
//             //Create a new instance of HttpClient
//             using (HttpClient client = new HttpClient())
//             //Setting up the response...         
//             using (HttpResponseMessage res = await client.GetAsync(baseUrl))
//             using (HttpContent content = res.Content){
//                 string data = await content.ReadAsStringAsync();
//                 // JObject data = await content.ReadAsAsync<JObject>();
//                 //Console.WriteLine(data);
//                 JObject json = JObject.Parse(data);
//                 JArray j  = ((JArray)json["results"]["bindings"]);
//                 if (data != null){
//                     Console.WriteLine(data);
//                     // var GeneratedSubject = ((JArray)json["results"]["bindings"]).Select(s => s["cidLabel"]["value"].ToString());
//                     // var GeneratedPredicate = ((JArray)json["results"]["bindings"]).Select(s => s["authortitleLabel"]["value"].ToString());
//                     for(int i=0; i < j.Count ; i++){ 
//                         universal_object s_universe = new universal_object(); 
//                         //Console.WriteLine("---"+(j[i]["cidLabel"]["value"].ToString()));
//                         s_universe.mainobject = (j[i]["cidLabel"]["value"].ToString());
//                         //Console.WriteLine("---"+(j[i]["authortitleLabel"]["value"]));
//                         s_universe.predicate = (string)j[i]["authortitleLabel"]["value"];
//                         universal_list_objects.Add(s_universe);
//                     }
                    
//                     return universal_list_objects;
//                 }
//                 return new List<universal_object>();
//             }
//         }
//         async Task<List<string>> GetQuestionData(string sparQL){
//             Console.WriteLine("---Inside-GetQuestionData---");
//             List<string> quesReviewList = new List<string>();
//             string baseUrl = "https://query.wikidata.org/sparql?query="+sparQL+"&format=json";
//             //The 'using' will help to prevent memory leaks.
//             //Create a new instance of HttpClient
//             using (HttpClient client = new HttpClient())
//             //Setting up the response...         
//             using (HttpResponseMessage res = await client.GetAsync(baseUrl))
//             using (HttpContent content = res.Content){
//                 string data = await content.ReadAsStringAsync();
//                 // JObject data = await content.ReadAsAsync<JObject>();
//                 JObject json = JObject.Parse(data);
//                 if (data != null){
//                     var GeneratedQuestions = ((JArray)json["results"]["bindings"]).Select(s => s["personLabel"]["value"].ToString());    // Nishant
//                     // for(int i=0; i < ((JArray)json["results"]["bindings"]).Count ; i++){  
//                     //     quesReviewList.Add(json["results"]["bindings"][i]["personLabel"]["value"].ToString());
//                     // }
//                     quesReviewList = GeneratedQuestions.ToList<string>();
//                 }
//                 return quesReviewList;  // Nishant
//             }
//         }
//         public bool GenerateOptions(QuizRTTemplate q) { Console.WriteLine("---Inside-GenerateOptions---");
//             string sparQL = "SELECT ?cid ?options WHERE {?cid wdt:P31 wd:Q28640. OPTIONAL {?cid rdfs:label ?options filter (lang(?options) = 'en') . }}Limit "+NumberOfQuestions*10+"";
//             Task<List<string>> dataReturns = System.Threading.Tasks.Task<string>.Run(() => GetOptionData(sparQL).Result);
//             List<string> optionReviewList = dataReturns.Result;
            
//             List<Questions> qL = context.QuestionsT
//                                         .Where( n => n.Categ == q.CategName)
//                                         .ToList();
            
//             for(int i=0; i<qL.Count; i++){
//                 List<Options> oL = new List<Options>();
//                 oL = randomizeOptions(optionReviewList, q.CategName, qL[i].QuestionsId);
//                 context.OptionsT.AddRange(oL);
//             }
//             context.SaveChanges();
//             return true;
//         }
//         async Task<List<string>> GetOptionData(string sparQL){ Console.WriteLine("---Inside-GetOptionData---");
//             List<string> optionReviewList = new List<string>();
//             string baseUrl = "https://query.wikidata.org/sparql?query="+sparQL+"&format=json";
//             //The 'using' will help to prevent memory leaks.
//             //Create a new instance of HttpClient
//             using (HttpClient client = new HttpClient())
//             //Setting up the response...         
//             using (HttpResponseMessage res = await client.GetAsync(baseUrl))
//             using (HttpContent content = res.Content){
//                 string data = await content.ReadAsStringAsync();
//                 // JObject data = await content.ReadAsAsync<JObject>();
//                 JObject json = JObject.Parse(data);
//                 // JArray J = (JArray)json["results"]["bindings"];
//                 if (data != null){
//                     // var GeneratedOptions = ((JArray)json["results"]["bindings"]).Select(s => s["options"]["value"].ToString());
//                     for(int i=0; i < ((JArray)json["results"]["bindings"]).Count ; i++){
//                         if ( ((JArray)json["results"]["bindings"])[i].Count() >= 2)
//                             optionReviewList.Add(json["results"]["bindings"][i]["options"]["value"].ToString());
//                     }
//                     return optionReviewList;
//                 }
//                 return new List<string>();
//             }
//         }
//         public List<Options> randomizeOptions(List<string> optionReviewList, string categName, int qId){
//             List<int> randomNumber = getRandonNumber(0, optionReviewList.Count-1, optionNumber-1);

//             List<Options> optionPerQues = new List<Options>();
//             for(int i=0; i < randomNumber.Count ; i++) {
//                 // if(optionReviewList[i] == categName){
//                     Options ops = new Options();
//                     ops.OptionGiven = optionReviewList[randomNumber[i]];
//                     ops.IsCorrect = false;
//                     ops.QuestionsId = qId;
//                     optionPerQues.Add(ops);
//                 // } else {
//                 //     randomizeOptions(optionReviewList, categName, qId);
//                 // }
//             }
//             Options opsCorrect = new Options();
//             opsCorrect.OptionGiven = categName;
//             opsCorrect.IsCorrect = true;
//             opsCorrect.QuestionsId = qId;
//             optionPerQues.Add(opsCorrect);
//             // shuffling the option to create randomness
//             // optionPerQues = shuffle(optionPerQues);
//             return optionPerQues;
//         }
//         public List<int> getRandonNumber(int iFromNum, int iToNum, int iNumOfItem){
//             List<int> lstNumbers = new List<int>();
//             Random rndNumber = new Random();

//             int number = rndNumber.Next(iFromNum, iToNum + 1);
//             lstNumbers.Add(number);
//             int count = 0;
//             do{
//                 number = rndNumber.Next(iFromNum, iToNum + 1);
//                 if (!lstNumbers.Contains(number)){
//                     lstNumbers.Add(number);
//                     count++;
//                 }
//             } while (count < iNumOfItem);
//             return lstNumbers;
//         }
//         public List<Options> shuffle(List<Options> optionPerQues){

//             return optionPerQues;
//         }

        // ---------------------New-Genearations------------------------
        // ----------------------After MongoDB--------------------------

        // public bool PostQueryNew(QuizRTTemplate qT){
        //     Console.WriteLine("---Inside-PostQuery---");
        //     if( context.QuizRTTemplateT.FirstOrDefault( n => n.Text == qT.Text) == null ){
        //         context.QuizRTTemplateT.Add(qT);
        //         context.SaveChanges();
        //         Console.WriteLine("---Template-Table-Inserted---");

        //         if ( GenerateQuestionNew(qT) ) 
        //             return true;
        //     }
        //     return false;
        // }

        // public bool GenerateQuestionNew(QuizRTTemplate q) {
        //     Console.WriteLine("---Inside-GenerateQuestion---");
        //     Console.WriteLine(q.Topic+";;;;;;;;;;;"+q.Categ);  


        //     string sparQL = "SELECT ?cidLabel ?authortitleLabel WHERE {?cid wdt:P31 wd:"+q.Topic+".?cid wdt:"+q.Categ+" ?authortitle .SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . }}LIMIT "+NumberOfQuestions+"";
        //     // string sparQL2 = $@"SELECT ?personLabel WHERE {{ ?person wdt:{q.Topic} wd:{q.Categ} . SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . } }LIMIT "+NumberOfQuestions+""; // Nishant
        //     Task<List<universal_object>> dataReturns = System.Threading.Tasks.Task<string>.Run(() => GetQuestionData_others(sparQL).Result);
        //     List<universal_object> quesReviewList = dataReturns.Result;
        //     Console.WriteLine(quesReviewList.Count+",,,,,,,,,,,,,,,,,,,,,,,,");
        //     List<Questions> qL = new List<Questions>();
        //     for(int i=0; i<quesReviewList.Count; i++){
        //         Console.WriteLine("------VVVVVVV------");

        //         string replacementStrSubject = GetBetween(q.Text,"[","]");
        //         string replacementStrObject = GetBetween(q.Text,"(",")");
        //         q.Text = Regex.Replace(q.Text, replacementStrObject, quesReviewList[i].predicate);
        //         q.Text = Regex.Replace(q.Text, replacementStrSubject, quesReviewList[i].mainobject);
        //         Console.WriteLine(q.Text);

        //         Questions ques = new Questions();
        //         ques.QuestionGiven = q.Text;
        //         ques.Topic = q.TopicName;
        //         ques.Categ = q.CategName;
        //         qL.Add(ques);
        //     }
        //     context.QuestionsT.AddRange(qL);
        //     context.SaveChanges();
        //     return true;
        // }

        // public static string GetBetween(string strSource, string strStart, string strEnd) {
        //     int Start, End;
        //     if (strSource.Contains(strStart) && strSource.Contains(strEnd)) {
        //         Start = strSource.IndexOf(strStart, 0) + strStart.Length;
        //         End = strSource.IndexOf(strEnd, Start);
        //         return strSource.Substring(Start, End - Start);
        //     }
        //     else {
        //         return "";
        //     }
        // }

    }

}