using System;
using System.Collections.Generic;
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
        public int regionId = 204;
        public int bookingTypeId = 2;
        public int specializationId = 9;
        public int clinicId = -1;
        public int languageId = -1;
        public int doctorId = -1;
        public DateTime searchSince = DateTime.Now.Date.AddDays(1);
        public string searchForNextSince = null;
        public int periodOfTheDay = 0;
        public bool isSetBecauseOfPcc = false;
        public bool isSetBecausePromoteSpecialization = false;
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
        public string id;
        public DateTime appointmentDate;
        public string specializationName;
        public string doctorName;
        public string clinicName;
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