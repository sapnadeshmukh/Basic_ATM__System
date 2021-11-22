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
        string myConnectionstring = "Data Source=.;Initial Catalog=ATM;Integrated Security=true";




        int Pin = 2222, CurrentAmount = 15000;





        //To check Balance
        [Route("api/CheckBalance/[controller]")]

        [HttpPost]

        public int BalanceShow([FromBody] int pin)
        {
            if (pin == Pin)
            {
                return CurrentAmount;
            }
            return 0;



        }




        //To deposit
        [Route("/ToDeposit")]
        [HttpPost]
        public int Deposit([FromBody] int amount)
        {
            string payment = "deposit";
            string querytoget = @"select top 1 TotalBalance from ATM_Users order by id desc";
            DataTable table1 = new DataTable();
            using (var con = new SqlConnection(myConnectionstring))
            using (var cmd = new SqlCommand(querytoget, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(table1);
            }
            int Balance = CurrentAmount;

            //Check if table is null/blank
            if (table1.Rows.Count > 0)
                Balance = Convert.ToInt32(table1.Rows[0][0]);
            //Main balance
            Balance =  (Balance + amount);
            string query = @"insert into dbo.ATM_Users values  (" + amount + @",'" + payment + @"','" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss") + "'," + Balance + ")";
            DataTable table = new DataTable();
            using (var con = new SqlConnection(myConnectionstring))
            using (var cmd = new SqlCommand(query, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(table);
            }
            return Balance;
        }



        // To withdraw
        [Route("/ToWithdraw")]
        [HttpPost]
        public int Withdraw([FromBody] int amount)
        {
            string payment = "Withdraw";
            string querytoget = @"select top 1 TotalBalance from ATM_Users order by id desc";
            DataTable table1 = new DataTable();
            using (var con = new SqlConnection(myConnectionstring))
            using (var cmd = new SqlCommand(querytoget, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(table1);
            }
            int Balance = CurrentAmount;

            //Check if table is null/blank
            if (table1.Rows.Count > 0)
                Balance = Convert.ToInt32(table1.Rows[0][0]);
            //Main balance
            Balance = (Balance - amount);
            string query = @"insert into dbo.ATM_Users values  (" + amount + @",'" + payment + @"','" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss") + "'," + Balance + ")";
            DataTable table = new DataTable();
            using (var con = new SqlConnection(myConnectionstring))
            using (var cmd = new SqlCommand(query, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(table);
            }
            return Balance;
        }


        //To check transactions
        [Route("/ToCheckTransactions")]
        [HttpGet]
        public object TransactionsDetails()
        {
            string query = @"select top 5 * from ATM_Users order by id desc ";
            DataTable table = new DataTable();
            using (var con = new SqlConnection(myConnectionstring))
            using (var cmd = new SqlCommand(query, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(table);
            }

            return new { data = table, success = true };


        }





    }



    
}
    