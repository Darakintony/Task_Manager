using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Management.Model;

namespace Task_Management.Service
{
    public interface IEmailService
    {
        void SendEmail(Message message);
        
    }
}
