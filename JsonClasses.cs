using System;
using System.Collections.Generic;
using System.Linq;
namespace meditool
{

    public class Config
    {
        public string UserName;
        public string Password;
        public int CheckIntervalMinutes = 5;
        public int DoNotSendPushForSlotsAboveDays = 14;
        public bool UsePushOver = true;
        public int  AfterHour = 0;

        public string pushOverUserId;
        public string pushOverAppTokenId;
        public SearchVisit_Konsultacja ConsultationSearchData;
        public SearchVisit_Badanie ExaminationSearchData;
        public PfmSearch PfmSearchData;
    }
    public class SearchVisit_Konsultacja
    {
        public int[] regionIds = {204};
        //public int bookingTypeId = 2;
        public int[] serviceIds = {9};

        public string[] selectedSpecialties = {};
        public int[] clinicIds = {};
        public int[] doctorLanguagesIds = {};
        public string[] doctorIds = {};
        public int serviceTypeId = 2;
        public DateTime searchSince = DateTime.Now.Date.AddDays(1);
        //public string searchForNextSince = null;
        //public int periodOfTheDay = 0;
        //public bool isSetBecauseOfPcc = false;
        //public bool isSetBecausePromoteSpecialization = false;
        public string startTime = "0:00";
        public string endtime = "23:59";
        public string visitType = "0";
    }

    public class SearchVisit_Badanie
    {
        public int regionId = 204;
        public int bookingTypeId = 1;
        public int serviceId = 521;
        public int clinicId = -1;
        public int languageId = -1;
        public int doctorId = -1;
        public DateTime searchSince = DateTime.Now.Date.AddDays(1);
        public string searchForNextSince = null;
        public int periodOfTheDay = 0;
        public bool isSetBecauseOfPcc = false;
        public bool isSetBecausePromoteSpecialization = false;
    }

    public class ConsultationsFound
    {
        public List<ConsultationFound> items = new List<ConsultationFound>();
    }
    public class ConsultationFound
    {
        public string id { get; set; } 
        public DateTime appointmentDate { get; set; } 
        public int serviceId { get; set; } 
        public string specializationName { get; set; } 
        public string doctorName { get; set; } 
        public int doctorId { get; set; } 
        public int doctorScheduleId { get; set; } 
        public int specialtyId { get; set; } 
        public int sysVisitTypeId { get; set; } 
        public string vendorTypeId { get; set; } 
        public List<object> doctorLanguages { get; set; } 
        public int clinicId { get; set; } 
        public string clinicName { get; set; } 
        public bool isPhoneConsultation { get; set; } 
        public bool isBloodCollectionPointConsultation { get; set; } 
    }
    public class PfmSession
    {
        public string id;
        public string code;
        public string settings;
    }
    public class AppointmentData
    {
        public string specialist;
        public string region;
        public string origin;
    }

    public class AdditionalSolutionsData
    {
    }
    public class AppointmentData2
    {
        public string caseId { get; set; }
        public string ticket { get; set; }
        public List<object> appointmentSlots { get; set; }
        public List<string> additionalSolutions { get; set; }
        public AdditionalSolutionsData additionalSolutionsData { get; set; }
        public List<string> specialties { get; set; }
        public List<string> vendors { get; set; }
    }
    public class PfmSearch1
    {
        public string caseId { get; set; }
        public string region { get; set; }
        public object clinic { get; set; }
        public object doctor { get; set; }
        public DateTime date { get; set; }
        public object slot { get; set; }
    }
    public class PfmSearch2
    {
        public string caseId { get; set; }
        public string region { get; set; }
        public object clinic { get; set; }
        public object doctor { get; set; }
        public DateTime date { get; set; }
        public object slot { get; set; }
        public string ticket;
    }
    public class PfmDictionaryItem
    {
        public string code;
        public string label;
    }
    public class AppointmentSlot
    {
        public string id { get; set; }
        public DateTime dateTime { get; set; }
        public string clinic { get; set; }
        public string specialty { get; set; }
        public string doctor { get; set; }
    }

    public class ConsultationFoundInternalV2
    {
        public string caseId { get; set; }
        public string ticket { get; set; }
        public List<AppointmentSlot> appointmentSlots { get; set; }
        public List<string> additionalSolutions { get; set; }
        public AdditionalSolutionsData additionalSolutionsData { get; set; }
    }
    public class PfmSearch
    {
        public int regionId { get; set; }
        public string MeetingString { get; set; }
        public string surveyName { get; set; }
        public string surveyresponse { get; set; }
        public object clinic { get; set; }
        public object doctor { get; set; }
        public DateTime date { get; set; }
    }
        public class DoctorInfo
    {
        public string DoctorName;
        public string Restrictions;
        public string Specialities;
        public string Rank;
        public string SurveyCount;
        public string Place;
    }

}