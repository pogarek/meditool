using System;
using System.Collections.Generic;
namespace meditool
{
    
    public class Config {
        public string UserName;
        public string Password;
        public int CheckIntervalMinutes = 5;
        public int DoNotSendPushForSlotsAboveDays = 14;

        public string pushOverUserId;
        public string pushOverAppTokenId;
        public SearchVisit_Konsultacja ConsultationSearchData;
        public SearchVisit_Badanie ExamindationSearchData;
    }
    public class SearchVisit_Konsultacja {
            public int regionId = 204;
            public int bookingTypeId = 2;
            public int specializationId = 9;
            public int clinicId = -1;
            public int languageId = -1;
            public int doctorId = -1;
            public DateTime searchSince = DateTime.Now.Date.AddDays(1);
            public string searchForNextSince =null;
            public int  periodOfTheDay = 0;
            public bool isSetBecauseOfPcc = false;
            public bool isSetBecausePromoteSpecialization = false;
    }

    public class SearchVisit_Badanie {
            public int regionId = 204;
            public int bookingTypeId = 1;
            public int serviceId = 521;
            public int clinicId = -1;
            public int languageId = -1;
            public int doctorId = -1;
            public DateTime searchSince = DateTime.Now.Date.AddDays(1);
            public string searchForNextSince =null;
            public int  periodOfTheDay = 0;
            public bool isSetBecauseOfPcc = false;
            public bool isSetBecausePromoteSpecialization = false;
    }

    public class ConsultationsFound {
        public List<ConsultationFound> items = new List<ConsultationFound>();
    }
    public class ConsultationFound {
            public string id ;
            public DateTime appointmentDate;
            public string specializationName;
            public string doctorName;
            public string clinicName;
    }
}