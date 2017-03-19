using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolicyBot.Models;

namespace PolicyBot
{
    public class PolicyDataController
    {
        public Policy GetPolicy(string policyKey)
        {
            var reply = new Policy();
            switch (policyKey.ToLower())
            {
                case "leave": reply = this.GetLeavePolicy();
                    break;
                case "casual":
                case "casual leave": 
                    reply = this.GetCausalLeavePolicy();
                    break;
                default: reply = null;
                    break;
            }
            return reply;
        }

        private Policy GetLeavePolicy()
        {
            var policyReply = new Policy();
            List<string> subLeavePolicies = 
                new List<string> { "sick leave", "casual leave", "privilege leave", "LOP" };
            policyReply.policy = "This is the leave policy";
            policyReply.subpolicies = subLeavePolicies;
            return policyReply;
        }

        private Policy GetCausalLeavePolicy()
        {
            var policyReply = new Policy();            
            policyReply.policy = "This is the casual leave policy";
            return policyReply;
        }
    }
}