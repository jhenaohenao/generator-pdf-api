﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formatos.Pdf.Core.Interfaces
{
    public interface IPdfService
    {
        byte[] ConvertHtmlToPdf(string htmlContent);
    }
}
