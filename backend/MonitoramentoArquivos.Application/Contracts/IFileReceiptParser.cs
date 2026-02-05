using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoramentoArquivos.Domain.Entities;

namespace MonitoramentoArquivos.Application.Contracts
{
    public interface IFileReceiptParser
    {
        FileReceipt Parse(string line, string fileName, string fileHash);
    }
}