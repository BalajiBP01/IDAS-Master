using Inspirit.IDAS.Data.Production;
using Inspirit.IDAS.Data.Raw;
using Inspirit.IDAS.ESData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.ETLApplication
{
    public class ES
    {

        public async Task Process()
        {
            RawDbContext rawContext = DbHelper.GetRawDbContext();
            ProductionDbContext prodContext = DbHelper.GetProductionDbContext();

            //ProcessTelephone(rawContext, prodContext);
            //ProcessConsumer(rawContext, prodContext);           
            //ProcessCommercial(rawContext, prodContext);

            //ProcessDirector(rawContext, prodContext);
            //ProcessPropertyDeed(rawContext, prodContext);
            //ProcessAuditor(rawContext, prodContext);

            await UpdateConsumerDataES(prodContext);
        }

        private async Task UpdateConsumerDataES(ProductionDbContext prodContext)
        {
            ESService es = new ESService();

            while (true)
            {
                //var consumers = prodContext.Consumers.Include(m => m.ConsumerEmails).Where(m => m.IsESSynced == false).Take(1000).ToList();
                var consumers = prodContext.Consumers.ToList();
                if (consumers.Count == 0) ;//break;

                List<ConsumerData> cdlist = new List<ConsumerData>();
                foreach (var prodCons in consumers)
                {
                    ConsumerData data = new ConsumerData();
                    data.ConsumerId = prodCons.ConsumerID.ToString();
                    data.Firstname = prodCons.FirstName;
                    data.Surname = prodCons.Surname;
                    data.IDNumber = prodCons.IDNO;
                    // data.MaritalStatus = prodCons.MaritalStatus.ToString();
                    data.Gender = prodCons.GenderInd.ToString();
                    data.DateOfBirth = prodCons.BirthDate.Value;
                    cdlist.Add(data);
                    // prodCons.IsESSynced = true;
                    prodContext.Update(prodCons);
                }
                es.UpsertConsumer(cdlist);
                await prodContext.SaveChangesAsync();

            }

        }

        private async Task UpdateCommercialDataES(ProductionDbContext prodContext)
        {
            ESService es = new ESService();

            while (true)
            {
                //var consumers = prodContext.Consumers.Include(m => m.ConsumerEmails).Where(m => m.IsESSynced == false).Take(1000).ToList();
                var commercial = prodContext.Commercials.ToList();
                //if (commercial.Count == 0) ;//break;

                List<CommercialData> cdlist = new List<CommercialData>();
                foreach (var prodCons in commercial)
                {
                    CommercialData data = new CommercialData();
                    data.CommercialID = prodCons.CommercialID;
                    data.CompanyName = prodCons.CommercialName;
                     data.CompanyRegNumber = prodCons.RegistrationNo;
                    //   data.IDNumber = prodCons.IDNO;
                    // data.MaritalStatus = prodCons.MaritalStatus.ToString();
                    //  data.Gender = prodCons.GenderInd.ToString();
                    // data.DateOfBirth = prodCons.BirthDate.Value;
                    cdlist.Add(data);
                    // prodCons.IsESSynced = true;
                    prodContext.Update(prodCons);
                }
                es.UpsertCommerical(cdlist);
                await prodContext.SaveChangesAsync();

            }

        }

        public void ProcessConsumer(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.Consumer> RawConsList = rawContext.Consumer.ToList();
            int count = 0;
            foreach (Data.Raw.Consumer rawCons in RawConsList)
            {
                if (IsRawConsumerValid(rawCons))
                {
                    Data.Production.Consumer prodCons = prodContext.Consumers.FirstOrDefault(
                        t => t.ConsumerID == rawCons.ConsumerID);

                    bool CreateNew = false;
                    if (prodCons == null)
                    {
                        prodCons = new Data.Production.Consumer();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawCons, prodCons);

                    if (CreateNew)
                    {
                        prodContext.Consumers.Add(prodCons);
                    }
                    else
                    {
                        prodContext.Consumers.Update(prodCons);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();

            ProcessHomeAffairs(rawContext, prodContext);
            ProcessConsumerAddress(rawContext, prodContext);
            ProcessConsumerEmploymentOccupation(rawContext, prodContext);
            ProcessConsumerTelephone(rawContext, prodContext);
            ProcessConsumerName(rawContext, prodContext);
            ProcessConsumerEmailConfirmed(rawContext, prodContext);
            ProcessConsumerDebtReview(rawContext, prodContext);
            ProcessConsumerJudgement(rawContext, prodContext);
        }

        public bool IsRawConsumerValid(Data.Raw.Consumer cons)
        {
            if (Util.IsConsumerIdValid(cons.IDNo) == false) return false;

            return true;
        }

        public void ProcessConsumerAddress(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.ConsumerAddress> RawConsAddrList = rawContext.ConsumerAddress.ToList();
            int count = 0;
            foreach (Data.Raw.ConsumerAddress rawConsAddr in RawConsAddrList)
            {
                if (IsRawConsumerAddressValid(rawConsAddr))
                {
                    Data.Production.ConsumerAddress prodConsAddr = prodContext.ConsumerAddress.FirstOrDefault(
                        t => t.ConsumerAddressID == rawConsAddr.ConsumerAddressID);

                    bool CreateNew = false;
                    if (prodConsAddr == null)
                    {
                        prodConsAddr = new Data.Production.ConsumerAddress();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawConsAddr, prodConsAddr);

                    if (CreateNew)
                    {
                        prodContext.ConsumerAddress.Add(prodConsAddr);
                    }
                    else
                    {
                        prodContext.ConsumerAddress.Update(prodConsAddr);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawConsumerAddressValid(Data.Raw.ConsumerAddress ConsAddr)
        {
            return true;
        }

        public void ProcessHomeAffairs(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.HomeAffairs> RawHomeAffList = rawContext.HomeAffairs.ToList();
            int count = 0;
            foreach (Data.Raw.HomeAffairs rawHomeAff in RawHomeAffList)
            {
                if (IsRawHomeAffairsValid(rawHomeAff))
                {
                    Data.Production.ConsumerHomeAffair prodHomeAff = prodContext.ConsumerHomeAffairs.FirstOrDefault(
                        t => t.HomeAffairsID == rawHomeAff.HomeAffairsID);

                    bool CreateNew = false;
                    if (prodHomeAff == null)
                    {
                        prodHomeAff = new Data.Production.ConsumerHomeAffair();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawHomeAff, prodHomeAff);

                    if (CreateNew)
                    {
                        prodContext.ConsumerHomeAffairs.Add(prodHomeAff);
                    }
                    else
                    {
                        prodContext.ConsumerHomeAffairs.Update(prodHomeAff);
                    }

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawHomeAffairsValid(Data.Raw.HomeAffairs HomeAff)
        {
            return true;
        }

        public void ProcessConsumerEmploymentOccupation(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.ConsumerEmploymentOccupation> RawConsEmpOccupList = rawContext.ConsumerEmploymentOccupation.ToList();
            int count = 0;
            foreach (Data.Raw.ConsumerEmploymentOccupation rawConsEmpOccup in RawConsEmpOccupList)
            {
                if (IsRawConsumerEmploymentOccupationValid(rawConsEmpOccup))
                {
                    Data.Production.ConsumerEmploymentOccupation prodConsEmpOccup = prodContext.ConsumerEmploymentOccupation.FirstOrDefault(
                        t => t.ConsumerEmploymentOccupationID == rawConsEmpOccup.ConsumerEmploymentOccupationID);

                    bool CreateNew = false;
                    if (prodConsEmpOccup == null)
                    {
                        prodConsEmpOccup = new Data.Production.ConsumerEmploymentOccupation();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawConsEmpOccup, prodConsEmpOccup);

                    Data.Raw.ConsumerEmployment RawConsEmp = rawContext.ConsumerEmployment.FirstOrDefault(t => t.ConsumerEmploymentID == prodConsEmpOccup.ConsumerEmploymentID);
                    if (RawConsEmp != null)
                    {
                        prodConsEmpOccup.employer = RawConsEmp.EmployerDetail;
                    }

                    if (CreateNew)
                    {
                        prodContext.ConsumerEmploymentOccupation.Add(prodConsEmpOccup);
                    }
                    else
                    {
                        prodContext.ConsumerEmploymentOccupation.Update(prodConsEmpOccup);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawConsumerEmploymentOccupationValid(Data.Raw.ConsumerEmploymentOccupation ConsEmp)
        {
            return true;
        }

        public void ProcessConsumerTelephone(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.ConsumerTelephone> RawConsTelList = rawContext.ConsumerTelephone.ToList();
            int count = 0;
            foreach (Data.Raw.ConsumerTelephone rawConsTel in RawConsTelList)
            {
                if (IsRawConsumerTelephoneValid(rawConsTel))
                {
                    Data.Production.ConsumerTelephone prodConsTel = prodContext.ConsumerTelephones.FirstOrDefault(
                        t => t.ConsumerTelephoneID == rawConsTel.ConsumerTelephoneID);

                    bool CreateNew = false;
                    if (prodConsTel == null)
                    {
                        prodConsTel = new Data.Production.ConsumerTelephone();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawConsTel, prodConsTel);

                    if (CreateNew)
                    {
                        prodContext.ConsumerTelephones.Add(prodConsTel);
                    }
                    else
                    {
                        prodContext.ConsumerTelephones.Update(prodConsTel);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawConsumerTelephoneValid(Data.Raw.ConsumerTelephone ConsTel)
        {
            return true;
        }

        public void ProcessConsumerName(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.ConsumerName> RawConsNameList = rawContext.ConsumerName.ToList();
            int count = 0;
            foreach (Data.Raw.ConsumerName rawConsName in RawConsNameList)
            {
                if (IsRawConsumerNameValid(rawConsName))
                {
                    Data.Production.Consumer prodCons = prodContext.Consumers.FirstOrDefault(
                        t => t.ConsumerID == rawConsName.ConsumerID);

                    bool CreateNew = false;
                    if (prodCons == null)
                    {
                        prodCons = new Data.Production.Consumer();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawConsName, prodCons);

                    if (CreateNew)
                    {
                        prodContext.Consumers.Add(prodCons);
                    }
                    else
                    {
                        //Only update if latest record
                        if (rawConsName.LastUpdatedDate > prodCons.LastUpdatedDate)
                        {
                            prodContext.Consumers.Update(prodCons);
                        }
                        else
                        {
                            // Log the details...
                        }
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawConsumerNameValid(Data.Raw.ConsumerName ConsName)
        {
            return true;
        }

        public void ProcessConsumerEmailConfirmed(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.ConsumerEmailConfirmed> RawConsEmailList = rawContext.ConsumerEmailConfirmed.ToList();
            int count = 0;
            foreach (Data.Raw.ConsumerEmailConfirmed rawConsEmail in RawConsEmailList)
            {
                if (IsRawConsumerEmailConfirmedValid(rawConsEmail))
                {
                    Data.Production.ConsumerEmailConfirmed prodConsEmail = prodContext.ConsumerEmails.FirstOrDefault(
                        t => t.ID == rawConsEmail.ID);

                    bool CreateNew = false;
                    if (prodConsEmail == null)
                    {
                        prodConsEmail = new Data.Production.ConsumerEmailConfirmed();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawConsEmail, prodConsEmail);

                    if (CreateNew)
                    {
                        prodContext.ConsumerEmails.Add(prodConsEmail);
                    }
                    else
                    {
                        prodContext.ConsumerEmails.Update(prodConsEmail);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawConsumerEmailConfirmedValid(Data.Raw.ConsumerEmailConfirmed ConsEmail)
        {
            return true;
        }


        public void ProcessConsumerDebtReview(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.ConsumerDebtReview> RawConsDebtRevList = rawContext.ConsumerDebtReview.ToList();
            int count = 0;
            foreach (Data.Raw.ConsumerDebtReview rawConsDebtRev in RawConsDebtRevList)
            {
                if (IsRawConsumerDebtReviewValid(rawConsDebtRev))
                {
                    Data.Production.ConsumerDebtReview prodConsDebtRev = prodContext.ConsumerDebtReviews.FirstOrDefault(
                        t => t.ConsumerDebtReviewID == rawConsDebtRev.ConsumerDebtReviewID);

                    bool CreateNew = false;
                    if (prodConsDebtRev == null)
                    {
                        prodConsDebtRev = new Data.Production.ConsumerDebtReview();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawConsDebtRev, prodConsDebtRev);

                    if (CreateNew)
                    {
                        prodContext.ConsumerDebtReviews.Add(prodConsDebtRev);
                    }
                    else
                    {
                        prodContext.ConsumerDebtReviews.Update(prodConsDebtRev);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawConsumerDebtReviewValid(Data.Raw.ConsumerDebtReview ConsDebtRev)
        {
            return true;
        }


        public void ProcessConsumerJudgement(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.ConsumerJudgement> RawConsJudgList = rawContext.ConsumerJudgement.ToList();
            int count = 0;
            foreach (Data.Raw.ConsumerJudgement rawConsJudg in RawConsJudgList)
            {
                if (IsRawConsumerJudgementValid(rawConsJudg))
                {
                    Data.Production.ConsumerJudgement prodConsJudg = prodContext.ConsumerJudgements.FirstOrDefault(
                        t => t.ConsumerJudgementID == rawConsJudg.ConsumerJudgementID);

                    bool CreateNew = false;
                    if (prodConsJudg == null)
                    {
                        prodConsJudg = new Data.Production.ConsumerJudgement();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawConsJudg, prodConsJudg);

                    if (CreateNew)
                    {
                        prodContext.ConsumerJudgements.Add(prodConsJudg);
                    }
                    else
                    {
                        prodContext.ConsumerJudgements.Update(prodConsJudg);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawConsumerJudgementValid(Data.Raw.ConsumerJudgement consJudg)
        {
            return true;
        }


        public void ProcessTelephone(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.Telephone> RawTelList = rawContext.Telephone.ToList();
            int count = 0;
            foreach (Data.Raw.Telephone rawTel in RawTelList)
            {
                if (IsRawTelephoneValid(rawTel))
                {
                    Data.Production.Telephone prodTel = prodContext.Telephones.FirstOrDefault(
                        t => t.TelephoneID == rawTel.TelephoneID);

                    bool CreateNew = false;
                    if (prodTel == null)
                    {
                        prodTel = new Data.Production.Telephone();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawTel, prodTel);

                    if (CreateNew)
                    {
                        prodContext.Telephones.Add(prodTel);
                    }
                    else
                    {
                        prodContext.Telephones.Update(prodTel);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawTelephoneValid(Data.Raw.Telephone Tel)
        {
            return true;
        }



        public void ProcessCommercial(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.Commercial> RawCommList = rawContext.Commercial.ToList();
            int count = 0;
            foreach (Data.Raw.Commercial rawComm in RawCommList)
            {
                if (IsRawCommercialValid(rawComm))
                {
                    Data.Production.Commercial prodComm = prodContext.Commercials.FirstOrDefault(
                        t => t.CommercialID == rawComm.CommercialID);

                    bool CreateNew = false;
                    if (prodComm == null)
                    {
                        prodComm = new Data.Production.Commercial();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawComm, prodComm);

                    if (CreateNew)
                    {
                        prodContext.Commercials.Add(prodComm);
                    }
                    else
                    {
                        prodContext.Commercials.Update(prodComm);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();

            ProcessCommercialName(rawContext, prodContext);
            ProcessCommercialTelephone(rawContext, prodContext);
            ProcessCommercialAddress(rawContext, prodContext);
            ProcessCommercialAuditor(rawContext, prodContext);
            ProcessCommercialDirector(rawContext, prodContext);
            ProcessCommercialJudgement(rawContext, prodContext);
            //ProcessCommercialCapital(rawContext, prodContext);
            //ProcessCommercialVatinfo(rawContext, prodContext);
        }

        public bool IsRawCommercialValid(Data.Raw.Commercial Comm)
        {
            return true;
        }

        public void ProcessCommercialAddress(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.CommercialAddress> RawCommAddrList = rawContext.CommercialAddress.ToList();
            int count = 0;
            foreach (Data.Raw.CommercialAddress rawCommAddr in RawCommAddrList)
            {
                if (IsRawCommercialAddressValid(rawCommAddr))
                {
                    Data.Production.CommercialAddress prodCommAddr = prodContext.CommercialAddress.FirstOrDefault(
                        t => t.CommercialAddressID == rawCommAddr.CommercialAddressID);

                    bool CreateNew = false;
                    if (prodCommAddr == null)
                    {
                        prodCommAddr = new Data.Production.CommercialAddress();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawCommAddr, prodCommAddr);

                    if (CreateNew)
                    {
                        prodContext.CommercialAddress.Add(prodCommAddr);
                    }
                    else
                    {
                        prodContext.CommercialAddress.Update(prodCommAddr);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawCommercialAddressValid(Data.Raw.CommercialAddress CommAddr)
        {
            return true;
        }

        public void ProcessCommercialAuditor(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.CommercialAuditor> RawCommAudList = rawContext.CommercialAuditor.ToList();
            int count = 0;
            foreach (Data.Raw.CommercialAuditor rawCommAud in RawCommAudList)
            {
                if (IsRawCommercialAuditorValid(rawCommAud))
                {
                    Data.Production.CommercialAuditor prodCommAud = prodContext.CommercialAuditor.FirstOrDefault(
                        t => t.CommercialAuditorID == rawCommAud.CommercialAuditorID);

                    bool CreateNew = false;
                    if (prodCommAud == null)
                    {
                        prodCommAud = new Data.Production.CommercialAuditor();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawCommAud, prodCommAud);

                    if (CreateNew)
                    {
                        prodContext.CommercialAuditor.Add(prodCommAud);
                    }
                    else
                    {
                        prodContext.CommercialAuditor.Update(prodCommAud);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawCommercialAuditorValid(Data.Raw.CommercialAuditor CommAud)
        {
            return true;
        }

        //public void ProcessCommercialCapital(RawDbContext rawContext, ProductionDbContext prodContext)
        //{
        //    List<Data.Raw.CommercialCapital> RawCommCapList = rawContext.CommercialCapital.ToList();
        //    int count = 0;
        //    foreach (Data.Raw.CommercialCapital rawCommCap in RawCommCapList)
        //    {
        //        if (IsRawCommercialCapitalValid(rawCommCap))
        //        {
        //            Data.Production.CommercialCapital prodCommCap = prodContext.CommercialCapital.FirstOrDefault(
        //                t => t.CommercialCapitalID == rawCommCap.CommercialCapitalID);

        //            bool CreateNew = false;
        //            if (prodCommCap == null)
        //            {
        //                prodCommCap = new Data.Production.CommercialCapital();
        //                CreateNew = true;
        //            }

        //            AutoMapper.Mapper.Map(rawCommCap, prodCommCap);

        //            if (CreateNew)
        //            {
        //                prodContext.CommercialCapital.Add(prodCommCap);
        //            }
        //            else
        //            {
        //                prodContext.CommercialCapital.Update(prodCommCap);
        //            }

        //            count++;

        //            if (count == 100)
        //            {
        //                prodContext.SaveChanges();
        //                count = 0;
        //            }
        //        }
        //        else
        //        {
        //            // Write to the log
        //        }
        //    }

        //    //If count < 100 at the end of for-each loop
        //    prodContext.SaveChanges();
        //}

        //public bool IsRawCommercialCapitalValid(Data.Raw.CommercialCapital CommCap)
        //{
        //    return true;
        //}

        public void ProcessCommercialDirector(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.CommercialDirector> RawCommDirList = rawContext.CommercialDirector.ToList();
            int count = 0;
            foreach (Data.Raw.CommercialDirector rawCommDir in RawCommDirList)
            {
                if (IsRawCommercialDirectorValid(rawCommDir))
                {
                    Data.Production.CommercialDirector prodCommDir = prodContext.CommercialDirector.FirstOrDefault(
                        t => t.CommercialDirectorID == rawCommDir.CommercialDirectorID);

                    bool CreateNew = false;
                    if (prodCommDir == null)
                    {
                        prodCommDir = new Data.Production.CommercialDirector();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawCommDir, prodCommDir);

                    if (CreateNew)
                    {
                        prodContext.CommercialDirector.Add(prodCommDir);
                    }
                    else
                    {
                        prodContext.CommercialDirector.Update(prodCommDir);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawCommercialDirectorValid(Data.Raw.CommercialDirector CommDir)
        {
            return true;
        }

        public void ProcessCommercialTelephone(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.CommercialTelephone> RawCommTelList = rawContext.CommercialTelephone.ToList();
            int count = 0;
            foreach (Data.Raw.CommercialTelephone rawCommTel in RawCommTelList)
            {
                if (IsRawCommercialTelephoneValid(rawCommTel))
                {
                    Data.Production.CommercialTelephone prodCommTel = prodContext.CommercialTelephone.FirstOrDefault(
                        t => t.CommercialTelephoneID == rawCommTel.CommercialTelephoneID);

                    bool CreateNew = false;
                    if (prodCommTel == null)
                    {
                        prodCommTel = new Data.Production.CommercialTelephone();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawCommTel, prodCommTel);

                    if (CreateNew)
                    {
                        prodContext.CommercialTelephone.Add(prodCommTel);
                    }
                    else
                    {
                        prodContext.CommercialTelephone.Update(prodCommTel);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawCommercialTelephoneValid(Data.Raw.CommercialTelephone CommTel)
        {
            return true;
        }


        public void ProcessCommercialName(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.CommercialName> RawCommNameList = rawContext.CommercialName.ToList();
            int count = 0;
            foreach (Data.Raw.CommercialName rawCommName in RawCommNameList)
            {
                if (IsRawCommercialNameValid(rawCommName))
                {
                    Data.Production.Commercial prodComm = prodContext.Commercials.FirstOrDefault(
                        t => t.CommercialID == rawCommName.CommercialID);

                    bool CreateNew = false;
                    if (prodComm == null)
                    {
                        prodComm = new Data.Production.Commercial();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawCommName, prodComm);

                    //This manual mapping required for the differences in name.
                    prodComm.CommercialName = rawCommName.CommName;

                    if (CreateNew)
                    {
                        prodContext.Commercials.Add(prodComm);
                    }
                    else
                    {
                        //Only update if latest record
                        if (rawCommName.LastUpdatedDate > prodComm.LastUpdatedDate)
                        {
                            prodContext.Commercials.Update(prodComm);
                        }
                        else
                        {
                            // Log the details...
                        }
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawCommercialNameValid(Data.Raw.CommercialName CommName)
        {
            return true;
        }

        public void ProcessCommercialJudgement(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            List<Data.Raw.CommercialJudgement> RawCommJudgList = rawContext.CommercialJudgement.ToList();
            int count = 0;
            foreach (Data.Raw.CommercialJudgement rawCommJudg in RawCommJudgList)
            {
                if (IsRawCommercialJudgementValid(rawCommJudg))
                {
                    Data.Production.CommercialJudgement prodCommJudg = prodContext.CommercialJudgement.FirstOrDefault(
                        t => t.CommercialJudgmentID == rawCommJudg.CommercialJudgmentID);

                    bool CreateNew = false;
                    if (prodCommJudg == null)
                    {
                        prodCommJudg = new Data.Production.CommercialJudgement();
                        CreateNew = true;
                    }

                    AutoMapper.Mapper.Map(rawCommJudg, prodCommJudg);

                    if (CreateNew)
                    {
                        prodContext.CommercialJudgement.Add(prodCommJudg);
                    }
                    else
                    {
                        prodContext.CommercialJudgement.Update(prodCommJudg);
                    }

                    count++;

                    if (count == 100)
                    {
                        prodContext.SaveChanges();
                        count = 0;
                    }
                }
                else
                {
                    // Write to the log
                }
            }

            //If count < 100 at the end of for-each loop
            prodContext.SaveChanges();
        }

        public bool IsRawCommercialJudgementValid(Data.Raw.CommercialJudgement CommJudg)
        {
            return true;
        }

        private void ProcessAuditor(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            throw new NotImplementedException();
        }

        private void ProcessPropertyDeed(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            throw new NotImplementedException();
        }

        private void ProcessDirector(RawDbContext rawContext, ProductionDbContext prodContext)
        {
            throw new NotImplementedException();
        }

    }
}
