﻿using Newtonsoft.Json;
using System;
using System.Data;

namespace getMotherDetails
{
    public class AncDetails
    {
        private readonly HelperClass _helper = new HelperClass();
        public void GetAncDetails(DataTable dtWsSyncDetails)
        {
            if (dtWsSyncDetails != null && dtWsSyncDetails.Rows[0]["GetANCDetails"].ToString() == "1")
            {
                try
                {
                    var dt = new GVK_ANCDetails._102Integration();
                    string data = dt.Get_ANCDetails("KCRKIT", "102@KCRKIT");
                    string updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now(),currentStatus = 'GetANCDetails' where isactive=1;";
                    _helper.ExecuteInsertStatement(updatestmt);
                    _helper.TraceService_result(data);
                    var dta = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));
                    if (dta == null || dta.Rows.Count <= 0)
                    {
                        _helper.TraceService_abnormal("GetANCDetails --Count is less than or equal to zero");
                        _helper.PushPostEventDetails(dtWsSyncDetails);
                    }
                    else
                    {
                        int count = dta.Rows.Count;
                        int count2 = Convert.ToInt32(dta.Rows[count - 1]["Count"].ToString());
                        if (count2 == 0)
                        {
                            _helper.TraceService_abnormal("GetANCDetails -- Count is or equal to zero");
                            _helper.PushPostEventDetails(dtWsSyncDetails);
                        }

                        if (count == count2 + 1)
                        {
                            _helper.TraceService("counts matched for batch id:" + dta.Rows[count - 1]["BatchId"].ToString());
                            _helper.InsertUpdateAncDetails(dta, count);
                            var us = new GVK_UPDATE_SyncDetails._102Integration();
                            var res = us.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "1");
                            _helper.TraceService("Response after updation for batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);
                        }
                        else
                        {
                            _helper.TraceService_abnormal("Counts are not matched for the batch with batch id " + dta.Rows[count - 1]["BatchId"].ToString());
                            var us = new GVK_UPDATE_SyncDetails._102Integration();
                            string res = us.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString(), "0");
                            _helper.TraceService("Response after updation for failed batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);
                        }
                        GetAncDetails(dtWsSyncDetails);
                    }
                }
                catch (Exception ex)
                {
                    _helper.TraceService_abnormal(ex.ToString());
                }
            }
            else
            {
                _helper.PushPostEventDetails(dtWsSyncDetails);
            }
        }
    }
}
