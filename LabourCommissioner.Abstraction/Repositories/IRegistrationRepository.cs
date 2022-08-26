﻿using LabourCommissioner.Abstraction.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabourCommissioner.Abstraction.Repositories
{
    public interface IRegistrationRepository :IBaseRepository<Registration>
    {
        Task<Registration> AddUpdateRegistration(Registration registration);
    }
}
