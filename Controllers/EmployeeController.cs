using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Net;

namespace Latest.Controllers
{
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        public static int Pin = 2222;
        Response resp = new Response();
        //To check Balance
        [Route("api/CheckBalance/[controller]")]

        [HttpPost]

        public Response BalanceShow([FromBody] int pin)
        {
            resp.Status = false;
            if (pin == Pin)
            {
                int Balance = EmployeeAction.GetBalance();
                if (Balance > 0)
                {
                    resp.Status = true;
                    resp.Balance = Balance;
                    resp.Message = "Your account balance is Rs "+ Balance+".";
                }
                else
                {
                    resp.Message ="No ";
                }
            }
            else
                resp.Message = "Sorry ! your pin is not currect.";
            return resp;
        }


        //To deposit
        [Route("/ToDeposit")]
        [HttpPost]
        public Response Deposit(Users data)
        {
            resp.Status = false;
            if (data.pin == Pin)
            {
                string payment = "deposit";
                int Balance = EmployeeAction.GetBalance();
                Balance = Balance + data.amount;
                string commandText = @"insert into dbo.ATM_Users values  (" + data.amount + @",'" + payment + @"','" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss") + "'," + Balance + ")";
                bool chk = EmployeeAction.RunQuery(commandText);
                if (chk)
                {
                    resp.Status = true;
                    resp.Balance = Balance;
                    resp.Message = "Successfully deposited.";
                }
                else
                    resp.Message = "Something going wrong!";
            }
            else
                resp.Message = "Sorry ! your pin is not currect.";

            return resp;
        }

        // To withdraw
        [Route("/ToWithdraw")]
        [HttpPost]
        public Response Withdraw([FromBody] Users data)
        {
            if (data.pin == Pin)
            {
                string payment = "withdraw";
                int Balance = EmployeeAction.GetBalance();
                if (Balance > data.amount)
                {
                    Balance = (Balance - data.amount);
                    string commandText = @"insert into dbo.ATM_Users values  (" + data.amount + @",'" + payment + @"','" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss") + "'," + Balance + ")";
                    bool chk = EmployeeAction.RunQuery(commandText);
                    if (chk)
                    {
                        resp.Status = true;
                        resp.Balance = Balance;
                        resp.Message = "Successfully withdrawal.";
                    }
                    else
                        resp.Message = "Something going wrong!";
                }
                else
                {
                    resp.Message = "Sorry ! you have insufficient balance.";
                }
            }
            else
                resp.Message = "Sorry ! your pin is not currect.";

            return resp;
        }


        //To check transactions
        [Route("/ToCheckTransactions")]
        [HttpGet]
        public Response TransactionsDetails()
        {
            string query = @"select top 5 price,payment,Timing,TotalBalance from ATM_Users order by id desc ";
            DataTable table = new DataTable();
            table = EmployeeAction.GetData(query);
            if (table.Rows.Count > 0)
            {
                resp.Balance = table;
                resp.Message = "Data found";
                resp.Status = true;
            }
            else
            {
                resp.Message = "No Data found";
                resp.Status = false;
            }
            return resp;
        }
    }



    public class Response
    {
        public Response()
        {
        }
        public bool Status { get; set; }
        public string Message { get; set; }
        public dynamic Balance { get; set; }
    }
    public class Users
    {
        public int amount { get; set; }
        public int pin { get; set; }
    }
    public class EmployeeAction
    {
        public static string myConnectionstring = "Data Source=.;Initial Catalog=ATM;Integrated Security=true";
        public static int CurrentAmount = 0;
        public static int GetBalance()
        {
            string querytoget = @"select top 1 TotalBalance from ATM_Users order by id desc";
            DataTable table = new DataTable();
            using (var con = new SqlConnection(myConnectionstring))
            using (var cmd = new SqlCommand(querytoget, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(table);
            }
            int Balance = CurrentAmount;

            //Check if table is null/blank
            if (table.Rows.Count > 0)
                Balance = Convert.ToInt32(table.Rows[0][0]);

            return Balance;
        }
        public static bool RunQuery(string commandText)
        {
            bool chk = false;
            using (SqlConnection conn = new SqlConnection(myConnectionstring))
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                chk = true;
                conn.Close();
            }
            return chk;
        }
        public static DataTable GetData(string commandText)
        {
            DataTable table = new DataTable();
            using (var con = new SqlConnection(myConnectionstring))
            using (var cmd = new SqlCommand(commandText, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(table);
            }
            return table;
        }
    }
}
