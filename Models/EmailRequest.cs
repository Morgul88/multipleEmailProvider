﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multipleEmailProvider.Models;

public class EmailRequest
{
    public List<string> To { get; set; }

    public string Subject { get; set; } = null!;

    public string HtmlBody { get; set; } = null!;

    public string PlainText { get; set; } = null!;
}
