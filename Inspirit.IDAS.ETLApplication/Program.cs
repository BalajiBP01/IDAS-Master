using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Raw = Inspirit.IDAS.Data.Raw;
using Production = Inspirit.IDAS.Data.Production;

namespace Inspirit.IDAS.ETLApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AutoMapper.Mapper.Initialize(cfg => 
            {
                cfg.CreateMap<Raw.Consumer, Production.Consumer>();
                cfg.CreateMap<Raw.ConsumerAddress, Production.ConsumerAddress>();
                cfg.CreateMap<Raw.ConsumerDebtReview, Production.ConsumerDebtReview>();
                cfg.CreateMap<Raw.ConsumerEmailConfirmed, Production.ConsumerEmailConfirmed>();
                cfg.CreateMap<Raw.ConsumerEmploymentOccupation, Production.ConsumerEmploymentOccupation>();
                cfg.CreateMap<Raw.ConsumerJudgement, Production.ConsumerJudgement>();
                cfg.CreateMap<Raw.ConsumerName, Production.Consumer>();
                cfg.CreateMap<Raw.ConsumerTelephone, Production.ConsumerTelephone>();
                cfg.CreateMap<Raw.HomeAffairs, Production.ConsumerHomeAffair>();

                cfg.CreateMap<Raw.Commercial, Production.Commercial>();
                cfg.CreateMap<Raw.CommercialAddress, Production.CommercialAddress>();
                cfg.CreateMap<Raw.CommercialAuditor, Production.CommercialAuditor>();
                cfg.CreateMap<Raw.CommercialDirector, Production.CommercialDirector>();
                cfg.CreateMap<Raw.CommercialJudgement, Production.CommercialJudgement>();
                cfg.CreateMap<Raw.CommercialName, Production.Commercial>();
                cfg.CreateMap<Raw.CommercialTelephone, Production.CommercialTelephone>();

                cfg.CreateMap<Raw.Director, Production.Director>();
                cfg.CreateMap<Raw.DirectorAddress, Production.DirectorAddress>();
                cfg.CreateMap<Raw.DirectorTelephone, Production.DirectorTelephone>();
                cfg.CreateMap<Raw.DirectorName, Production.Director>();

                cfg.CreateMap<Raw.PropertyDeed, Production.PropertyDeed>();
                cfg.CreateMap<Raw.Buyer, Production.PropertyDeedBuyer>(); 
                cfg.CreateMap<Raw.Seller, Production.PropertyDeedSeller>();

                cfg.CreateMap<Raw.Auditor, Production.Auditor>();
                cfg.CreateMap<Raw.AuditorAddress, Production.AuditorAddress>();
                cfg.CreateMap<Raw.AuditorHistory, Production.AuditorHistory>();
            });

            //.ForMember(x => x.BuyerID, opt => opt.Ignore())

            Application.Run(new FormETL());
        }
    }
}
