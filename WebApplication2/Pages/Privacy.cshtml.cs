using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
CREATE TABLE[dbo].[Assets] (
    [Id]                  INT NOT NULL,
    [object_name]         TEXT         NULL,
    [inventory_number]    INT          NULL,
    [quantity]            INT          NULL,
    [year_introduction]   INT          NULL,
    [initial_cost]        DECIMAL (18) NULL,
    [residual_value]      DECIMAL (18) NULL,
    [useful_life]         INT          NULL,
    [technical_condition] TEXT         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE[dbo].[Users] (
    [Id]          INT NOT NULL,
    [fullname]    NVARCHAR (MAX) NULL,
    [email]       NVARCHAR (MAX) NULL,
    [password]    NVARCHAR (MAX) NULL,
    [phonenumber] DECIMAL (18)   NULL,
    [role]        NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


