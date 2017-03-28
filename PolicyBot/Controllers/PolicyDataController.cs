using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolicyBot.Models;
using PolicyBot.Externals;
using System.Threading.Tasks;

namespace PolicyBot
{
    public class PolicyDataController
    {
        public async Task<Policy> GetPolicy(string policyKey)
        {
            Policy reply;
            MongoConnection _db;
            try
            {
                reply = new Policy();
                if (policyKey.ToLower() == "none")
                {
                    reply.policyText = "I din't understand that. Please provide a different query (Sorry, I am still learning)";
                }
                else
                {
                    _db = new MongoConnection();
                    reply = await _db.GetPolicyDetails(policyKey);
                }
                #region old code
                /*
                switch (policyKey.ToLower())
                {
                    
                    case "leave":
                        reply = this.GetLeavePolicy();
                        break;
                    case "casual":
                    case "casual leave":
                        _db = new MongoConnection();
                        reply = await _db.GetPolicyDetails("casual_leave");
                        break;
                    case "sick":
                    case "sick leave":
                        reply = this.GetSickLeavePolicy();
                        break;
                    case "none":
                        reply.policyText = "I din't understand that. Please provide a different query (Sorry, I am still learning)";
                        break;
                    default:
                        reply = null;
                        break;
                        
                }*/
                #endregion
            }
            catch (Exception)
            {
                reply = null;
            }
            finally
            {
                _db = null;
            }
            return reply;
        }
        #region to remove
        private Policy GetLeavePolicy()
        {
            var policyReply = new Policy();
            List<string> subLeavePolicies =
                new List<string> { "sick leave", "casual leave", "privilege leave", "LOP" };
            policyReply.policyText = "This is the leave policy";
            policyReply.subpolicies = subLeavePolicies;
            return policyReply;
        }

        private Policy GetCausalLeavePolicy()
        {
            var policyReply = new Policy();
            policyReply.policyText = "This is the casual leave policy";
            return policyReply;
        }

        private Policy GetSickLeavePolicy()
        {
            var policyReply = new Policy();
            policyReply.policyText = "This is the sick leave policy";
            return policyReply;
        }
        #endregion
    }
}