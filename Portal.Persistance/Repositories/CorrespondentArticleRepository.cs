﻿using Portal.Data.Entities;
using Portal.Persistance.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Persistance.Repositories
{
    public class CorrespondentArticleRepository : Repository<CorrespondentArticle>, ICorrespondentArticleRepository
    {

        public CorrespondentArticleRepository(PortalDbContext context)
            : base(context)
        {
        }
        public PortalDbContext Context { get { return Context as PortalDbContext; } }
    }
}