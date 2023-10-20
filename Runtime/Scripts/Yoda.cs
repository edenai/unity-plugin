using System.Collections.Generic;
using System;

namespace EdenAI
{
    [Serializable]
    public class YodaResponse
    {
        public string result { get; set; }
    }
    public class YodaRequest
    {
        public List<Dictionary<string, string>> history { get; set; }
        public int? k { get; set; }
        public string llm_model { get; set; }
        public string llm_provider { get; set; }
        public string query { get; set; }
        
        public YodaRequest(string query, List<Dictionary<string, string>> history = null, int? k = null, string llmModel = null,
            string llmProvider = null)
        {
            this.query = query;
            this.history = history;
            this.k = k;
            this.llm_model = llmModel;
            this.llm_provider = llmProvider;
        }
    }
}