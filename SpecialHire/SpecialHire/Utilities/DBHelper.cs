using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EbusDataProvider;
using SpecialHire.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data;

namespace SpecialHire.Utilities
{
    public class DBHelper
    {
        public CommonHelper commonHelper = new CommonHelper();
        public CompanyConfigurationInfo ConfigurationSettings
        {
            get
            {
                return (HttpContext.Current.Session["CONFIGURATION"] != null) ? (CompanyConfigurationInfo)(CompanyConfigurationInfo)HttpContext.Current.Session["CONFIGURATION"] : new CompanyConfigurationInfo();
            }
        }

        public List<BookingApprovalInfo> SearchPendingBookings(string bookingDate, string bookingID)
        {
            var filterDate = new DateTime();
            if (bookingDate != string.Empty)
            {
                filterDate = Convert.ToDateTime(bookingDate);
            }

            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from item in DBContext.BookingApprovalInfoes.AsEnumerable()
                            where (item.InvoiceNumber.ToLower().Contains(bookingID.ToLower()) || bookingID == string.Empty) &&
                                    (bookingDate == string.Empty || item.InvoiceDate.Value.Date.Equals(filterDate.Date))
                            orderby item.ID
                            select item).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateBookingStatus(int bookingID, string comment)
        {
            var response = true;
            using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
            {
                using (var dbContextTransaction = DBContext.Database.BeginTransaction())
                {
                    try
                    {

                        var booking = DBContext.BookingInfoes.SingleOrDefault(x => x.ID.Equals(bookingID));
                        if (booking != null)
                        {
                            booking.ApprovalComments = comment;
                            booking.IsBookingApproved = true;
                            booking.ModifiedBy = commonHelper.GetLoggedInUserName();
                            booking.ModifiedOn = DateTime.Now;
                        }
                        DBContext.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
            return response;
        }


        #region Login
        public ApplicationUser ValidateLogin(ApplicationUser applicationUser)
        {
            try
            {
                using (EBusAdministrationContext DBContext = new EBusAdministrationContext())
                {
                    return DBContext.ApplicationUsers.AsEnumerable().Where(e => e.UserName.Equals(applicationUser.UserName) && e.Password.Equals(applicationUser.Password) && e.Status == true).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public CompanyConfigurationInfo LoadConfigurationSettings(int companyID)
        {
            try
            {
                using (EBusAdministrationContext DBContext = new EBusAdministrationContext())
                {
                    return DBContext.CompanyConfigurationInfoes.Include("Company").Where(e => e.CompanyID.Equals(companyID) && e.Status == true).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Trailer Type
        public List<TrailerType> SearchTrailerTypes(string type)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from trailerType in DBContext.TrailerTypes
                            where trailerType.Type.ToLower().Contains(type.ToLower()) || type == string.Empty
                            select trailerType).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool SaveTrailerTypeInformation(TrailerTypeModel trailerTypeModal)
        {
            var response = true;
            using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
            {
                using (var dbContextTransaction = DBContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (trailerTypeModal.ID > 0)
                        {
                            response = UpdateTrailerTypeInformation(trailerTypeModal, DBContext);
                        }
                        else
                        {
                            response = CreateTrailerTypeInformation(trailerTypeModal, DBContext);
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
            return response;
        }

        public bool CheckTrailerTypeDependency(int trailerTypeID)
        {
            var today = DateTime.Now.Date;
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from trailer in DBContext.BookingTrailerInfoes
                            join quote in DBContext.BookingQuoteInfoes on trailer.QuotationID equals quote.ID
                            where (quote.PickUpDate >= today || quote.ReturnDate >= today)
                            && (trailer.TrailerTypeID.Equals(trailerTypeID))
                            select trailer).ToList().Count() > 0 ? true : false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CreateTrailerTypeInformation(TrailerTypeModel trailerTypeModal, SpecialHireDBContext dBContext)
        {
            var response = true;

            var item = dBContext.TrailerTypes.Where(t => t.Type == trailerTypeModal.Type).Count();
            if (item > 0) { return false; }

            dBContext.TrailerTypes.Add(new TrailerType()
            {
                ID = trailerTypeModal.ID,
                Type = trailerTypeModal.Type,
                MaxWeight = trailerTypeModal.MaxWeight,
                Description = trailerTypeModal.Description,
                Status = true,
                ModifiedBy = commonHelper.GetLoggedInUserName(),
                ModifiedOn = DateTime.Now
            });

            dBContext.SaveChanges();
            return response;
        }

        public bool UpdateTrailerTypeInformation(TrailerTypeModel trailerTypeModal, SpecialHireDBContext dBContext)
        {
            var response = true;
            var i = dBContext.BusTypes.Where(b => b.Type == trailerTypeModal.Type && b.ID != trailerTypeModal.ID).Count();
            if (i > 0) { return false; }

            var item = dBContext.TrailerTypes.SingleOrDefault(m => m.ID == trailerTypeModal.ID);

            item.Type = trailerTypeModal.Type;
            item.MaxWeight = trailerTypeModal.MaxWeight;
            item.Description = trailerTypeModal.Description;
            item.Status = trailerTypeModal.Status;
            item.ModifiedBy = commonHelper.GetLoggedInUserName();
            item.ModifiedOn = DateTime.Now;

            dBContext.SaveChanges();
            return response;
        }
        #endregion

        #region Trailer

        public List<TrailerModel> SearchTrailers(string fleetNumber, string vINNumber, int trailerTypeID)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from trailer in DBContext.Trailers
                            join trailerType in DBContext.TrailerTypes on trailer.TrailerTypeID equals trailerType.ID
                            where (trailer.TrailerTypeID.Equals(trailerTypeID) || trailerTypeID == 0) &&
                                    (trailer.FleetNumber.ToLower().Contains(fleetNumber.ToLower()) || fleetNumber == string.Empty) &&
                                    (trailer.VINNumber.ToLower().Contains(vINNumber.ToLower()) || vINNumber == string.Empty)
                            select new TrailerModel
                            {
                                ID = trailer.ID,
                                TrailerTypeID = trailer.TrailerTypeID,
                                FleetNumber = trailer.FleetNumber,
                                TrailerType = trailerType.Type,
                                VINNumber = trailer.VINNumber,
                                Registration = trailer.Registration,
                                Status = trailer.Status,
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveTrailerInformation(TrailerModel trailer)
        {
            var response = true;
            using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
            {
                using (var dbContextTransaction = DBContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (trailer.ID > 0)
                        {
                            response = UpdateTrailerInformation(trailer, DBContext);
                        }
                        else
                        {
                            response = CreateTrailerInformation(trailer, DBContext);
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
            return response;
        }

        public bool CreateTrailerInformation(TrailerModel trailer, SpecialHireDBContext dBContext)
        {
            var response = true;
            var item = dBContext.Trailers.Where(t => t.FleetNumber == trailer.FleetNumber || t.VINNumber == trailer.VINNumber || t.Registration == trailer.Registration).Count();
            if (item > 0) { return false; }

            dBContext.Trailers.Add(new Trailer()
            {
                VINNumber = trailer.VINNumber,
                FleetNumber = trailer.FleetNumber,
                Registration = trailer.Registration,
                TrailerTypeID = trailer.TrailerTypeID,
                Status = true,
                ModifiedBy = commonHelper.GetLoggedInUserName(),
                ModifiedOn = DateTime.Now
            });
            dBContext.SaveChanges();

            return response;
        }

        public bool UpdateTrailerInformation(TrailerModel trailer, SpecialHireDBContext dBContext)
        {
            var response = true;
            var i = dBContext.Trailers.Where(t => t.FleetNumber == trailer.FleetNumber || t.VINNumber == trailer.VINNumber || t.Registration == trailer.Registration && t.ID != trailer.ID).Count();
            if (i > 0) { return false; }

            var item = dBContext.Trailers.SingleOrDefault(m => m.ID == trailer.ID);
            item.VINNumber = trailer.VINNumber;
            item.FleetNumber = trailer.FleetNumber;
            item.Registration = trailer.Registration;
            item.TrailerTypeID = trailer.TrailerTypeID;
            item.Status = trailer.Status;
            item.ModifiedBy = commonHelper.GetLoggedInUserName();
            item.ModifiedOn = DateTime.Now;

            dBContext.SaveChanges();

            return response;
        }

        public bool CheckTrailerDependency(int trailerID)
        {
            var today = DateTime.Now.Date;
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from trailer in DBContext.BookingTrailerInfoes
                            join quote in DBContext.BookingQuoteInfoes on trailer.QuotationID equals quote.ID
                            where (quote.PickUpDate >= today || quote.ReturnDate >= today)
                            && (trailer.TrailerID.HasValue && trailer.TrailerID.Value.Equals(trailerID))
                            select trailer).ToList().Count() > 0 ? true : false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Bus
        public List<BusModel> SearchBusses(string fleetNumber, string vINNumber, int busTypeID)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from bus in DBContext.Buses
                            join busType in DBContext.BusTypes on bus.BusTypeID equals busType.ID
                            where (bus.BusTypeID.Equals(busTypeID) || busTypeID == 0) &&
                                    (bus.FleetNumber.ToLower().Contains(fleetNumber.ToLower()) || fleetNumber == string.Empty) &&
                                    (bus.VINNumber.ToLower().Contains(vINNumber.ToLower()) || vINNumber == string.Empty)
                            select new BusModel
                            {
                                ID = bus.ID,
                                BusTypeID = bus.BusTypeID,
                                BusType = busType.Type,
                                FleetNumber = bus.FleetNumber,
                                VINNumber = bus.VINNumber,
                                Registration = bus.Registration,
                                Status = bus.Status,
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveBusInformation(BusModel bus)
        {
            var response = true;
            using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
            {
                using (var dbContextTransaction = DBContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (bus.ID > 0)
                        {
                            response = UpdateBusInformation(bus, DBContext);
                        }
                        else
                        {
                            response = CreateBusInformation(bus, DBContext);
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
            return response;
        }

        public bool CreateBusInformation(BusModel bus, SpecialHireDBContext dBContext)
        {
            var response = true;
            var item = dBContext.Buses.Where(b => b.FleetNumber == bus.FleetNumber || b.VINNumber == bus.VINNumber || b.Registration == bus.Registration).Count();
            if (item > 0) { return false; }

            dBContext.Buses.Add(new Bus()
            {
                FleetNumber = bus.FleetNumber,
                VINNumber = bus.VINNumber,
                Registration = bus.Registration,
                BusTypeID = bus.BusTypeID,
                Status = true,
                ModifiedBy = commonHelper.GetLoggedInUserName(),
                ModifiedOn = DateTime.Now
            });
            dBContext.SaveChanges();

            return response;
        }

        public bool UpdateBusInformation(BusModel bus, SpecialHireDBContext dBContext)
        {
            var response = true;
            var i = dBContext.Buses.Where(b => b.FleetNumber == bus.FleetNumber || b.VINNumber == bus.VINNumber || b.Registration == bus.Registration && b.ID != bus.ID).Count();
            if (i > 0) { return false; }

            var item = dBContext.Buses.SingleOrDefault(m => m.ID == bus.ID);
            item.FleetNumber = bus.FleetNumber;
            item.VINNumber = bus.VINNumber;
            item.Registration = bus.Registration;
            item.BusTypeID = bus.BusTypeID;
            item.Status = bus.Status;
            item.ModifiedBy = commonHelper.GetLoggedInUserName();
            item.ModifiedOn = DateTime.Now;

            dBContext.SaveChanges();

            return response;
        }

        public bool checkBusDependency(int busID)
        {
            var today = DateTime.Now.Date;
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from vehicle in DBContext.BookingVehicleInfoes
                            join quote in DBContext.BookingQuoteInfoes on vehicle.QuotationID equals quote.ID
                            where (quote.PickUpDate >= today || quote.ReturnDate >= today)
                            && (vehicle.BusID.HasValue && vehicle.BusID.Value.Equals(busID))
                            select vehicle).ToList().Count() > 0 ? true : false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Bus Types
        public List<BusType> SearchBusTypes(string type)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from busType in DBContext.BusTypes
                            where busType.Type.ToLower().Contains(type.ToLower()) || type == string.Empty
                            select busType).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveBusTypeInformation(BusTypeModel busTypeModal)
        {
            var response = true;
            using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
            {
                using (var dbContextTransaction = DBContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (busTypeModal.ID > 0)
                        {
                            response = UpdateBusTypeInformation(busTypeModal, DBContext);
                        }
                        else
                        {
                            response = CreateBusTypeInformation(busTypeModal, DBContext);
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
            return response;
        }

        public bool CreateBusTypeInformation(BusTypeModel busTypeModal, SpecialHireDBContext dBContext)
        {
            var response = true;
            var item = dBContext.BusTypes.Where(b => b.Type == busTypeModal.Type).Count();
            if (item > 0) { return false; }

            dBContext.BusTypes.Add(new BusType()
            {
                ID = busTypeModal.ID,
                Type = busTypeModal.Type,
                Capacity = busTypeModal.Capacity,
                Standing = busTypeModal.Standing,
                Sitting = busTypeModal.Sitting,
                Description = busTypeModal.Description,
                AC = busTypeModal.AC,
                TV = busTypeModal.TV,
                Curtains = busTypeModal.Curtains,
                Toilet = busTypeModal.Toilet,
                Status = true,
                ModifiedBy = commonHelper.GetLoggedInUserName(),
                ModifiedOn = DateTime.Now
            });

            dBContext.SaveChanges();
            return response;
        }

        public bool UpdateBusTypeInformation(BusTypeModel busTypeModal, SpecialHireDBContext dBContext)
        {
            var response = true;
            var i = dBContext.BusTypes.Where(b => b.Type == busTypeModal.Type && b.ID != busTypeModal.ID).Count();
            if (i > 0) { return false; }

            var item = dBContext.BusTypes.SingleOrDefault(m => m.ID == busTypeModal.ID);

            item.Type = busTypeModal.Type;
            item.Capacity = busTypeModal.Capacity;
            item.Standing = busTypeModal.Standing;
            item.Sitting = busTypeModal.Sitting;
            item.Description = busTypeModal.Description;
            item.AC = busTypeModal.AC;
            item.TV = busTypeModal.TV;
            item.Curtains = busTypeModal.Curtains;
            item.Toilet = busTypeModal.Toilet;
            item.Status = busTypeModal.Status;
            item.ModifiedBy = commonHelper.GetLoggedInUserName();
            item.ModifiedOn = DateTime.Now;

            dBContext.SaveChanges();
            return response;
        }

        public bool CheckBusTypeDependency(int busTypeID)
        {
            var today = DateTime.Now.Date;
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from vehicle in DBContext.BookingVehicleInfoes
                            join quote in DBContext.BookingQuoteInfoes on vehicle.QuotationID equals quote.ID
                            where (quote.PickUpDate >= today || quote.ReturnDate >= today)
                            && (vehicle.BusTypeID.Equals(busTypeID))
                            select vehicle).ToList().Count() > 0 ? true : false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Booking Quote

        public BookingQuoteInfo GetBookingQuoteByID(int quotationID)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var quotation = (from quote in DBContext.BookingQuoteInfoes.AsEnumerable()
                                     where quote.ID.Equals(quotationID)
                                     select quote).FirstOrDefault();
                    return quotation;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<BookingQuoteInfo> SearchBookingQuotes(string quotationID, string phoneNumber, string name)
        {
            var resp = new List<BookingQuoteInfo>();
            var today = DateTime.Now.Date;
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    resp = (from quote in DBContext.BookingQuoteInfoes
                            orderby quote.PickUpDate
                            where (quote.AlternateID.Contains(quotationID) || quotationID == string.Empty) &&
                                    (quote.CellNumber.ToLower().Contains(phoneNumber) || phoneNumber == string.Empty) &&
                                    (quote.FirstName.ToLower().Contains(name.ToLower()) || name == string.Empty) &&
                                    DbFunctions.TruncateTime(quote.PickUpDate) >= today
                            select quote).ToList();
                    if (resp.Count == 0)
                    {
                        if (quotationID == string.Empty)
                        {
                            var defaultResp = (from quote in DBContext.BookingQuoteInfoes.AsEnumerable()
                                               orderby quote.PickUpDate
                                               where (quote.CellNumber.Contains(phoneNumber) || phoneNumber == string.Empty) &&
                                                       (quote.FirstName.ToLower().Contains(name.ToLower()) || name == string.Empty)
                                               select quote).FirstOrDefault();
                            if (defaultResp != null)
                            {
                                resp = new List<BookingQuoteInfo>();
                                resp.Add(defaultResp);
                            }
                        }
                    }

                    return resp;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<BookingVehicleInfoModel> GetVehicleDetailsByQuotationID(int quotationID)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var bookingVehicleInfo = (from vehicle in DBContext.BookingVehicleInfoes
                                              join BusType in DBContext.BusTypes on vehicle.BusTypeID equals BusType.ID
                                              where vehicle.QuotationID.Equals(quotationID)
                                              select new BookingVehicleInfoModel
                                              {
                                                  ID = vehicle.ID,
                                                  BusTypeID = vehicle.BusTypeID,
                                                  Capacity = BusType.Capacity,
                                                  Sitting = BusType.Sitting,
                                                  Standing = BusType.Standing,
                                                  Description = BusType.Description
                                              }).ToList();

                    return bookingVehicleInfo;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<BookingTrailerInfoModel> GetTrailerDetailsByQuotationID(int quotationID)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var bookingTrailerInfo = (from Trailer in DBContext.BookingTrailerInfoes
                                              join TrailerType in DBContext.TrailerTypes on Trailer.TrailerTypeID equals TrailerType.ID
                                              where Trailer.QuotationID.Equals(quotationID)
                                              select new BookingTrailerInfoModel
                                              {
                                                  ID = Trailer.ID,
                                                  TrailerTypeID = Trailer.TrailerTypeID,
                                                  MaxWeight = TrailerType.MaxWeight,
                                                  Description = TrailerType.Description
                                              }).ToList();

                    return bookingTrailerInfo;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public BookingInfo GetBookingByQuotationID(int quotationID)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var Booking = (from booking in DBContext.BookingInfoes.AsEnumerable()
                                   where booking.QuotationID.Equals(quotationID)
                                   select booking).FirstOrDefault();

                    return Booking;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public BusType GetBusTypeDetails(int busTypeID)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var busType = (from type in DBContext.BusTypes.AsEnumerable()
                                   where type.ID == busTypeID
                                   select type).FirstOrDefault();

                    return busType;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public CompanyDetail GetCompanyDetailsBasedOnID(int companyID)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var company = (from comp in DBContext.CompanyDetails.AsEnumerable()
                                   where comp.ID == companyID
                                   select comp).FirstOrDefault();

                    return company;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateInvoiceFileName(string fileName, int BookingID,string ConnectionKey)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConnectionKey))
                {
                    var item = (from type in DBContext.BookingInfoes
                                where type.ID == BookingID
                                select type).FirstOrDefault();

                    if (item != null)
                    {
                        item.InvoiceFileName = fileName;
                        DBContext.SaveChanges();
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public TrailerType GetTrailerTypeDetails(int trailerTypeID)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var trailerType = (from type in DBContext.TrailerTypes.AsEnumerable()
                                       where type.ID == trailerTypeID
                                       select type).FirstOrDefault();

                    return trailerType;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateQuotationFileName(string fileName, int QuotationID, string ConnectionKey)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConnectionKey))
                {
                    var item = (from type in DBContext.BookingQuoteInfoes
                                where type.ID == QuotationID
                                select type).FirstOrDefault();

                    if (item != null)
                    {
                        item.QuotationFileName = fileName;
                        DBContext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GenerateBookingQuote(ref BookingQuoteInfoModel bookingQuote)
        {
            using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
            {
                using (var dbContextTransaction = DBContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (bookingQuote.ID > 0)
                        {
                            UpdateBookingQuote(ref bookingQuote, DBContext);
                        }
                        else
                        {
                            CreateBookingQuote(ref bookingQuote, DBContext);
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void CreateBookingQuote(ref BookingQuoteInfoModel bookingQuote, SpecialHireDBContext DBContext)
        {
            var item = new BookingQuoteInfo();
            //item.ID = bookingQuote.ID;
            item.Title = bookingQuote.Title;
            item.FirstName = bookingQuote.FirstName;
            item.SurName = bookingQuote.SurName;
            item.TelephoneNumber = bookingQuote.TelephoneNumber;
            item.CellNumber = bookingQuote.CellNumber;
            item.EmailAddress = bookingQuote.EmailAddress;
            item.CompanyName = bookingQuote.CompanyName;
            item.Address = bookingQuote.Address;
            item.PostalCode = bookingQuote.PostalCode;
            item.CompTelephoneNumber = bookingQuote.CompTelephoneNumber;
            item.CompTelephoneExtension = bookingQuote.CompTelephoneExtension;
            item.FaxNumber = bookingQuote.FaxNumber;
            item.IsReturnJourney = bookingQuote.IsReturnJourney;
            item.IsSingleJourney = bookingQuote.IsSingleJourney;
            item.IsQuoteValidTill = bookingQuote.IsQuoteValidTillAdded;
            item.IsTrailerAdded = bookingQuote.IsTrailerRequired;
            item.PickUpDate = bookingQuote.PickUpDate;
            item.PickUpTime = bookingQuote.PickUpTime;
            item.ReturnDate = bookingQuote.ReturnDate;
            item.ReturnTime = bookingQuote.ReturnTime;
            item.FromLocation = bookingQuote.FromLocation;
            item.ToLocation = bookingQuote.ToLocation;
            item.Distance = bookingQuote.Distance;
            item.Passengers = bookingQuote.Passengers;
            item.EventID = bookingQuote.EventID;
            item.ExtraInformation = bookingQuote.ExtraInformation;
            item.PaymentTermsID = bookingQuote.PaymentTermsID;
            //if (bookingQuote.IsQuoteValidTillAdded) item.QuoteValidTill = bookingQuote.QuoteValidTill; else item.QuoteValidTill = (DateTime.Today.AddDays(ConfigurationSettings.QuotationValidityPeriod.Value));
            item.QuoteValidTill = bookingQuote.QuoteValidTill;
            item.QuotationValue = bookingQuote.QuotationValue;
            item.CompanyID = bookingQuote.CompanyID;
            item.Status = true;
            item.ModifiedBy = commonHelper.GetLoggedInUserName();
            item.ModifiedOn = DateTime.Now;

            DBContext.BookingQuoteInfoes.Add(item);
            DBContext.SaveChanges();
            item.AlternateID = bookingQuote.CompanyPrefix.Trim() + item.ID.ToString();
            DBContext.SaveChanges();

            bookingQuote.AlternateID = item.AlternateID;

            bookingQuote.ID = item.ID;

            if (bookingQuote.BookingVehicleInfo.Count > 0 && bookingQuote.BookingVehicleInfo != null)
            {
                var VehicleDetails = bookingQuote.BookingVehicleInfo;
                var newVehicleDetails = new List<BookingVehicleInfo>();
                foreach (BookingVehicleInfoModel vehicle in VehicleDetails)
                {
                    var newInfo = new BookingVehicleInfo();
                    newInfo.BusTypeID = vehicle.BusTypeID;
                    newInfo.QuotationID = item.ID;
                    newInfo.ModifiedBy = commonHelper.GetLoggedInUserName();
                    newInfo.ModifiedOn = DateTime.Now;
                    newVehicleDetails.Add(newInfo);
                }
                DBContext.BookingVehicleInfoes.AddRange(newVehicleDetails);
                DBContext.SaveChanges();
            }
            if (bookingQuote.IsTrailerRequired && bookingQuote.BookingTrailerInfo != null && bookingQuote.BookingTrailerInfo.Count > 0)
            {
                var TrailerDetails = bookingQuote.BookingTrailerInfo;
                var newTrailerDetails = new List<BookingTrailerInfo>();
                foreach (BookingTrailerInfoModel trailer in TrailerDetails)
                {
                    var newInfo = new BookingTrailerInfo();
                    newInfo.TrailerTypeID = trailer.TrailerTypeID;
                    newInfo.QuotationID = item.ID;
                    newInfo.ModifiedBy = commonHelper.GetLoggedInUserName();
                    newInfo.ModifiedOn = DateTime.Now;
                    newTrailerDetails.Add(newInfo);
                }
                DBContext.BookingTrailerInfoes.AddRange(newTrailerDetails);
                DBContext.SaveChanges();
            }
        }

        private void UpdateBookingQuote(ref BookingQuoteInfoModel bookingQuote, SpecialHireDBContext DBContext)
        {
            var QuotationID = bookingQuote.ID;
            var item = DBContext.BookingQuoteInfoes.SingleOrDefault(q => q.ID == QuotationID);
            if (item != null)
            {
                item.ID = bookingQuote.ID;
                item.Title = bookingQuote.Title;
                item.FirstName = bookingQuote.FirstName;
                item.SurName = bookingQuote.SurName;
                item.TelephoneNumber = bookingQuote.TelephoneNumber;
                item.CellNumber = bookingQuote.CellNumber;
                item.EmailAddress = bookingQuote.EmailAddress;
                item.CompanyName = bookingQuote.CompanyName;
                item.Address = bookingQuote.Address;
                item.PostalCode = bookingQuote.PostalCode;
                item.CompTelephoneNumber = bookingQuote.CompTelephoneNumber;
                item.CompTelephoneExtension = bookingQuote.CompTelephoneExtension;
                item.FaxNumber = bookingQuote.FaxNumber;
                item.IsReturnJourney = bookingQuote.IsReturnJourney;
                item.IsSingleJourney = bookingQuote.IsSingleJourney;
                item.IsQuoteValidTill = bookingQuote.IsQuoteValidTillAdded;
                item.IsTrailerAdded = bookingQuote.IsTrailerRequired;
                item.PickUpDate = bookingQuote.PickUpDate;
                item.PickUpTime = bookingQuote.PickUpTime;
                item.ReturnDate = bookingQuote.ReturnDate;
                item.ReturnTime = bookingQuote.ReturnTime;
                item.FromLocation = bookingQuote.FromLocation;
                item.ToLocation = bookingQuote.ToLocation;
                item.Distance = bookingQuote.Distance;
                item.Passengers = bookingQuote.Passengers;
                item.EventID = bookingQuote.EventID;
                item.ExtraInformation = bookingQuote.ExtraInformation;
                item.PaymentTermsID = bookingQuote.PaymentTermsID;
                //if (bookingQuote.IsQuoteValidTillAdded) item.QuoteValidTill = bookingQuote.QuoteValidTill; else item.QuoteValidTill = (DateTime.Today.AddDays(ConfigurationSettings.QuotationValidityPeriod.Value));
                item.QuoteValidTill = bookingQuote.QuoteValidTill;
                item.QuotationValue = bookingQuote.QuotationValue;
                item.CompanyID = bookingQuote.CompanyID;
                item.ModifiedBy = commonHelper.GetLoggedInUserName();
                item.ModifiedOn = DateTime.Now;

                DBContext.SaveChanges();
            }

            if (bookingQuote.BookingVehicleInfo != null && bookingQuote.BookingVehicleInfo.Count > 0)
            {
                var delVehicleDetails = DBContext.BookingVehicleInfoes.Where(v => v.QuotationID == QuotationID);
                DBContext.BookingVehicleInfoes.RemoveRange(delVehicleDetails);

                var VehicleDetails = bookingQuote.BookingVehicleInfo;
                var newVehicleDetails = new List<BookingVehicleInfo>();
                foreach (BookingVehicleInfoModel vehicle in VehicleDetails)
                {
                    var newInfo = new BookingVehicleInfo();
                    newInfo.BusTypeID = vehicle.BusTypeID;
                    newInfo.QuotationID = bookingQuote.ID;
                    newInfo.ModifiedBy = commonHelper.GetLoggedInUserName();
                    newInfo.ModifiedOn = DateTime.Now;
                    newVehicleDetails.Add(newInfo);
                }
                DBContext.BookingVehicleInfoes.AddRange(newVehicleDetails);
                DBContext.SaveChanges();
            }

            if (bookingQuote.IsTrailerRequired && bookingQuote.BookingTrailerInfo != null && bookingQuote.BookingTrailerInfo.Count > 0)
            {
                var delTrailerDetails = DBContext.BookingTrailerInfoes.Where(v => v.QuotationID == QuotationID);
                DBContext.BookingTrailerInfoes.RemoveRange(delTrailerDetails);

                var TrailerDetails = bookingQuote.BookingTrailerInfo;
                var newTrailerDetails = new List<BookingTrailerInfo>();
                foreach (BookingTrailerInfoModel trailer in TrailerDetails)
                {
                    var newInfo = new BookingTrailerInfo();
                    newInfo.TrailerTypeID = trailer.TrailerTypeID;
                    newInfo.QuotationID = bookingQuote.ID;
                    newInfo.ModifiedBy = commonHelper.GetLoggedInUserName();
                    newInfo.ModifiedOn = DateTime.Now;
                    newTrailerDetails.Add(newInfo);
                }
                DBContext.BookingTrailerInfoes.AddRange(newTrailerDetails);
                DBContext.SaveChanges();
            }
            else
            {
                var delTrailerDetails = DBContext.BookingTrailerInfoes.Where(v => v.QuotationID == QuotationID);
                DBContext.BookingTrailerInfoes.RemoveRange(delTrailerDetails);
            }
        }

        public void GenerateBooking(ref BookingQuoteInfoModel bookingQuote)
        {
            using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
            {
                using (var dbContextTransaction = DBContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (bookingQuote.ID > 0)
                        {
                            UpdateBooking(ref bookingQuote, DBContext);
                        }
                        else
                        {
                            CreateBooking(ref bookingQuote, DBContext);
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void CreateBooking(ref BookingQuoteInfoModel bookingQuote, SpecialHireDBContext DBContext)
        {
            UpdateBookingQuote(ref bookingQuote, DBContext);
            var bookingInfo = bookingQuote.BookingInfo;
            if (bookingQuote.BookingInfo != null)
            {
                bookingInfo.QuotationID = bookingQuote.ID;
                bookingInfo.Status = true;
                bookingInfo.ModifiedBy = commonHelper.GetLoggedInUserName();
                bookingInfo.ModifiedOn = DateTime.Now;

                DBContext.BookingInfoes.Add(bookingInfo);
                DBContext.SaveChanges();
                bookingInfo.AlternateID = bookingQuote.CompanyPrefix.Trim() + bookingInfo.ID.ToString();
                DBContext.SaveChanges();
            }
        }

        private void UpdateBooking(ref BookingQuoteInfoModel bookingQuote, SpecialHireDBContext DBContext)
        {
            UpdateBookingQuote(ref bookingQuote, DBContext);
            if (bookingQuote.BookingInfo != null)
            {
                bookingQuote.BookingInfo.QuotationID = bookingQuote.ID;
                bookingQuote.BookingInfo.Status = true;
                bookingQuote.BookingInfo.ModifiedBy = commonHelper.GetLoggedInUserName();
                bookingQuote.BookingInfo.ModifiedOn = DateTime.Now;

                DBContext.BookingInfoes.Add(bookingQuote.BookingInfo);
                DBContext.SaveChanges();
                bookingQuote.BookingInfo.AlternateID = bookingQuote.CompanyPrefix.Trim() + bookingQuote.BookingInfo.ID.ToString();
                DBContext.SaveChanges();
            }
        }


        #endregion

        #region Master
        public List<SelectListItem> GetBusTypes()
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return commonHelper.AddDefaultItem((from type in DBContext.BusTypes.AsEnumerable()
                                                        where type.Status == true
                                                        select new SelectListItem { Text = type.Type, Value = type.ID.ToString() }).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> GetEvents()
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    DBContext.Configuration.LazyLoadingEnabled = true;
                    return commonHelper.AddDefaultItem((from e in DBContext.Events.AsEnumerable()
                                                        where e.Status == true
                                                        select new SelectListItem { Text = e.EventName, Value = e.ID.ToString() }).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> GetDrivers()
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return commonHelper.AddDefaultItem((from driver in DBContext.Drivers.AsEnumerable()
                                                        where driver.Status == true
                                                        select new SelectListItem { Text = driver.DriverName, Value = driver.ID.ToString() }).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> GetBuses()
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return commonHelper.AddDefaultItem((from bus in DBContext.Buses.AsEnumerable()
                                                        where bus.Status == true
                                                        select new SelectListItem { Text = bus.FleetNumber, Value = bus.ID.ToString() }).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> GetUserRoles()
        {
            try
            {
                using (EBusAdministrationContext DBContext = new EBusAdministrationContext())
                {
                    return commonHelper.AddDefaultItem((from role in DBContext.UserRoles.AsEnumerable()
                                                        where role.Status == true
                                                        select new SelectListItem { Text = role.Role, Value = role.ID.ToString() }).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> GetTrailerTypes()
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return commonHelper.AddDefaultItem((from trailer in DBContext.TrailerTypes.AsEnumerable()
                                                        where trailer.Status == true
                                                        select new SelectListItem { Text = trailer.Type, Value = trailer.ID.ToString() }).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> GetTrailers()
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return commonHelper.AddDefaultItem((from trailer in DBContext.Trailers.AsEnumerable()
                                                        where trailer.Status == true
                                                        select new SelectListItem { Text = trailer.Registration, Value = trailer.ID.ToString() }).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> GetPaymentModes()
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return commonHelper.AddDefaultItem((from mode in DBContext.PaymentModes.AsEnumerable()
                                                        where mode.Status == true
                                                        select new SelectListItem { Text = mode.Mode, Value = mode.ID.ToString() }).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> GetPaymentTerms()
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return commonHelper.AddDefaultItem((from terms in DBContext.PaymentTerms.AsEnumerable()
                                                        where terms.Status == true
                                                        select new SelectListItem { Text = terms.Terms, Value = terms.ID.ToString() }).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> GetCompanyDetails()
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return commonHelper.AddDefaultItem((from company in DBContext.CompanyDetails.AsEnumerable()
                                                        where company.Status == true
                                                        select new SelectListItem { Text = company.Company, Value = company.ID.ToString() }).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Dispatcher

        public List<DispatcherBookingInfoModel> SearchBookings(string bookingDate, string bookingID)
        {
            var filterDate = new DateTime();
            if (bookingDate != string.Empty)
            {
                filterDate = Convert.ToDateTime(bookingDate);
            }

            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from quote in DBContext.DispatcherBookingInfoes.AsEnumerable()
                            where (quote.AlternateID.ToLower().Contains(bookingID) || bookingID == string.Empty) &&
                                    (bookingDate == string.Empty || quote.PickUpDate.Equals(filterDate))
                            orderby quote.ID
                            select new DispatcherBookingInfoModel
                            {
                                BookingID = quote.BookingID,
                                BookingVehicleID = quote.BookingVehicleID,
                                CellNumber = quote.CellNumber,
                                OrganiserName = quote.OrganiserName,
                                FromLocation = quote.FromLocation,
                                NumberOfTrailers = quote.NumberOfTrailers.Value,
                                PickUpTime = quote.PickUpTime,
                                QuotationID = quote.ID,
                                ReturnTime = quote.ReturnTime,
                                ToLocation = quote.ToLocation,
                                IsJobCompleted = quote.IsJobCompleted,
                                IsVehicleAssigned = quote.IsVehicleAssigned,
                                IsTrailerAssigned = quote.IsTrailerAssigned,
                                NumberOfBusses = quote.NumberOfBusses.Value,
                                BusType = quote.BusType,
                                DriverName = quote.DriverName,
                                FleetNumber = quote.FleetNumber
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DispatcherVehicleInfoModel GetDispatcherVehicleInfo(int quotationID, int bookingVehicleID)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var dispatcherVehicleInfoModel = new DispatcherVehicleInfoModel();
                    DispatcherVehicleChecklist vehicleCheckList = null;
                    DispatcherVehicleCustomerSurvey customerSurvey = null;
                    List<int?> allBookedTrailerInfo = new List<int?>();
                    List<int?> allBookedVehicleInfo = new List<int?>();
                    List<int?> allBookedDriverInfo = new List<int?>();
                    List<int> remainingBusTypes = new List<int>();
                    List<int> remainingTrailerTypes = new List<int>();

                    var vehicleInfo = (from quote in DBContext.BookingQuoteInfoes
                                       join vehicle in DBContext.BookingVehicleInfoes
                                       on quote.ID equals vehicle.QuotationID
                                       join trailer in DBContext.BookingTrailerInfoes
                                       on vehicle.ID equals trailer.BookingVehicleID into joined
                                       from item in joined.DefaultIfEmpty()
                                       where quote.ID.Equals(quotationID) && vehicle.ID.Equals(bookingVehicleID)
                                       select new { quote, vehicle, item }).FirstOrDefault();

                    //if (vehicleInfo.vehicle.BusID == null)
                    //{
                    //    allBookedVehicleInfo = (from quote in DBContext.BookingQuoteInfoes
                    //                            join vehicle in DBContext.BookingVehicleInfoes
                    //                               on quote.ID equals vehicle.QuotationID
                    //                            where vehicle.BusID != null && quote.PickUpDate == vehicleInfo.quote.PickUpDate
                    //                            select (vehicle.BusID)).ToList();
                    //}
                    //else
                    //{
                    //    allBookedVehicleInfo = (from quote in DBContext.BookingQuoteInfoes
                    //                            join vehicle in DBContext.BookingVehicleInfoes
                    //                               on quote.ID equals vehicle.QuotationID
                    //                            where vehicle.BusID != null && quote.PickUpDate == vehicleInfo.quote.PickUpDate
                    //                            && vehicle.BusID != vehicleInfo.vehicle.BusID
                    //                            select (vehicle.BusID)).ToList();
                    //}

                    //if (vehicleInfo.item == null)
                    //{
                    //    allBookedTrailerInfo = (from quote in DBContext.BookingQuoteInfoes
                    //                            join item in DBContext.BookingTrailerInfoes
                    //                               on quote.ID equals item.QuotationID
                    //                            where item.TrailerID != null && quote.PickUpDate == vehicleInfo.quote.PickUpDate
                    //                            select (item.TrailerID)).ToList();
                    //}
                    //else {
                    //    allBookedTrailerInfo = (from quote in DBContext.BookingQuoteInfoes
                    //                            join item in DBContext.BookingTrailerInfoes
                    //                               on quote.ID equals item.QuotationID
                    //                            where item.TrailerID != null && quote.PickUpDate == vehicleInfo.quote.PickUpDate
                    //                            && item.TrailerID != vehicleInfo.item.TrailerID
                    //                            select (item.TrailerID)).ToList();
                    //}

                    //if (vehicleInfo.vehicle.DriverID == null)
                    //{
                    //    allBookedDriverInfo = (from quote in DBContext.BookingQuoteInfoes
                    //                           join vehicle in DBContext.BookingVehicleInfoes
                    //                              on quote.ID equals vehicle.QuotationID
                    //                           where vehicle.DriverID != null && quote.PickUpDate == vehicleInfo.quote.PickUpDate
                    //                           select (vehicle.DriverID)).ToList();
                    //}
                    //else
                    //{
                    //    allBookedDriverInfo = (from quote in DBContext.BookingQuoteInfoes
                    //                           join vehicle in DBContext.BookingVehicleInfoes
                    //                              on quote.ID equals vehicle.QuotationID
                    //                           where vehicle.DriverID != null && quote.PickUpDate == vehicleInfo.quote.PickUpDate
                    //                           && vehicle.DriverID != vehicleInfo.vehicle.DriverID
                    //                           select (vehicle.DriverID)).ToList();
                    //}


                    remainingBusTypes = (from vehicle in DBContext.BookingVehicleInfoes
                                         where vehicle.QuotationID.Equals(quotationID) && vehicle.BusID == null
                                         select vehicle.BusTypeID).ToList();

                    remainingTrailerTypes = (from trailer in DBContext.BookingTrailerInfoes
                                             where trailer.QuotationID.Equals(quotationID) && trailer.TrailerID == null
                                             select trailer.TrailerTypeID).ToList();

                    var busNumbers = (from bus in DBContext.Buses.AsEnumerable()
                                      where bus.Status == true// && !allBookedVehicleInfo.Contains(bus.ID)
                                      && (vehicleInfo.vehicle.IsJobCompleted || remainingBusTypes.Contains(bus.BusTypeID))
                                      select new SelectListItem
                                      {
                                          Text = bus.FleetNumber,
                                          Value = bus.ID.ToString()
                                      }).ToList();

                    var trailerNumbers = (from trailer in DBContext.Trailers.AsEnumerable()
                                          where trailer.Status == true// && !allBookedTrailerInfo.Contains(trailer.ID)
                                          && (vehicleInfo.vehicle.IsJobCompleted || remainingTrailerTypes.Contains(trailer.TrailerTypeID))
                                          select new SelectListItem
                                          {
                                              Text = trailer.FleetNumber,
                                              Value = trailer.ID.ToString()
                                          }).ToList();

                    var drivers = (from driver in DBContext.Drivers.AsEnumerable()
                                   where driver.IsSpecialHireDriver == true && driver.Status == true //&& !allBookedDriverInfo.Contains(driver.ID)
                                   select new SelectListItem
                                   {
                                       Text = driver.DriverName,
                                       Value = driver.ID.ToString()
                                   }).ToList();

                    if (vehicleInfo.vehicle.IsJobCompleted)
                    {
                        vehicleCheckList = (from info in DBContext.DispatcherVehicleChecklists
                                            where info.QuotationID == quotationID && info.BookingVehicleID == bookingVehicleID
                                            select info).FirstOrDefault();

                        customerSurvey = (from info in DBContext.DispatcherVehicleCustomerSurveys
                                          where info.QuotationID == quotationID && info.BookingVehicleID == bookingVehicleID
                                          select info).FirstOrDefault();
                    }


                    dispatcherVehicleInfoModel.BusNumbers = commonHelper.AddDefaultItem(busNumbers);
                    dispatcherVehicleInfoModel.TrailerNumbers = commonHelper.AddDefaultItem(trailerNumbers);
                    dispatcherVehicleInfoModel.Drivers = commonHelper.AddDefaultItem(drivers);
                    dispatcherVehicleInfoModel.BusID = (vehicleInfo.vehicle.BusID == null) ? 0 : vehicleInfo.vehicle.BusID.Value;
                    dispatcherVehicleInfoModel.DriverID = (vehicleInfo.vehicle.DriverID == null) ? 0 : vehicleInfo.vehicle.DriverID.Value;
                    dispatcherVehicleInfoModel.FromLocation = vehicleInfo.quote.FromLocation;
                    dispatcherVehicleInfoModel.ToLocation = vehicleInfo.quote.ToLocation;

                    if (vehicleInfo.item != null)
                    {
                        dispatcherVehicleInfoModel.TrailerID = (vehicleInfo.item.TrailerID == null) ? 0 : vehicleInfo.item.TrailerID.Value;
                    }

                    if (vehicleInfo.vehicle.IsJobCompleted)
                    {
                        if (vehicleCheckList != null)
                        {
                            dispatcherVehicleInfoModel.FullUniform = vehicleCheckList.FullUniform;
                            dispatcherVehicleInfoModel.BriefedPickUpPoint = vehicleCheckList.BriefedPickUpPoint;
                            dispatcherVehicleInfoModel.BriefedFinalDestination = vehicleCheckList.BriefedFinalDestination;
                            dispatcherVehicleInfoModel.Permit = vehicleCheckList.Permit;
                            dispatcherVehicleInfoModel.JobCardAndMaps = vehicleCheckList.JobCardAndMaps;
                            dispatcherVehicleInfoModel.RadioFace = vehicleCheckList.RadioFace;
                        }
                        if (customerSurvey != null)
                        {
                            dispatcherVehicleInfoModel.WasDrivingOnTime = customerSurvey.WasDrivingOnTime;
                            dispatcherVehicleInfoModel.WasDriverProperlyAttired = customerSurvey.WasDriverProperlyAttired;
                            dispatcherVehicleInfoModel.DidHeKnowTheRoute = customerSurvey.DidHeKnowTheRoute;
                            dispatcherVehicleInfoModel.DidHeAdhereToTrafficRules = customerSurvey.DidHeAdhereToTrafficRules;
                            dispatcherVehicleInfoModel.AreMapsAvailable = customerSurvey.AreMapsAvailable;
                            dispatcherVehicleInfoModel.CustomerComments = customerSurvey.CustomerComments;
                        }

                        dispatcherVehicleInfoModel.TimeOfLeavingDepot = (vehicleInfo.vehicle.TimeOfLeavingDepot == null) ? "0" : vehicleInfo.vehicle.TimeOfLeavingDepot;
                        dispatcherVehicleInfoModel.SpeedoReadingBeforeLeavingFromDepot = (vehicleInfo.vehicle.SpeedoReadingBefore == null) ? "" : vehicleInfo.vehicle.SpeedoReadingBefore;
                        dispatcherVehicleInfoModel.TimeOfReturnDepot = (vehicleInfo.vehicle.TimeOfReturnDepot == null) ? "0" : vehicleInfo.vehicle.TimeOfReturnDepot;
                        dispatcherVehicleInfoModel.SpeedoReadingReturnToDepot = (vehicleInfo.vehicle.SpeedoReadingReturn == null) ? "" : vehicleInfo.vehicle.SpeedoReadingReturn;
                        dispatcherVehicleInfoModel.CustomerName = vehicleInfo.quote.FirstName;
                    }
                    return dispatcherVehicleInfoModel;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveAssignedBusAndTrailer(DispatcherVehicleInfoModel dispatcherVehicleInfo)
        {
            using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
            {
                using (var dbContextTransaction = DBContext.Database.BeginTransaction())
                {
                    try
                    {
                        var userName = commonHelper.GetLoggedInUserName();

                        var vehicle = DBContext.BookingVehicleInfoes.SingleOrDefault(m => m.ID == dispatcherVehicleInfo.BookingVehicleID);
                        vehicle.BusID = dispatcherVehicleInfo.BusID;
                        vehicle.DriverID = dispatcherVehicleInfo.DriverID;
                        vehicle.IsVehicleAssigned = true;
                        vehicle.ModifiedBy = userName;
                        vehicle.ModifiedOn = DateTime.Now;

                        DBContext.SaveChanges();

                        if (dispatcherVehicleInfo.IsTrailerAssigned)
                        {
                            var trailer = DBContext.BookingTrailerInfoes.SingleOrDefault(m => m.QuotationID == dispatcherVehicleInfo.QuotationID &&
                               m.BookingVehicleID == dispatcherVehicleInfo.BookingVehicleID);

                            if (dispatcherVehicleInfo.TrailerID > 0)
                            {
                                trailer.TrailerID = dispatcherVehicleInfo.TrailerID;
                                trailer.BookingVehicleID = dispatcherVehicleInfo.BookingVehicleID;
                                trailer.IsTrailerAssigned = true;
                                trailer.ModifiedBy = userName;
                                trailer.ModifiedOn = DateTime.Now;
                            }
                            else
                            {
                                trailer.TrailerID = null;
                                trailer.BookingVehicleID = null;
                                trailer.IsTrailerAssigned = false;
                                trailer.ModifiedBy = userName;
                                trailer.ModifiedOn = DateTime.Now;
                            }
                            DBContext.SaveChanges();
                        }
                        else
                        {
                            if (dispatcherVehicleInfo.TrailerID > 0)
                            {
                                var trailerType = (DBContext.Trailers.SingleOrDefault(m => m.ID == dispatcherVehicleInfo.TrailerID)).TrailerTypeID;
                                var trailer = (DBContext.BookingTrailerInfoes.Where(m => m.QuotationID == dispatcherVehicleInfo.QuotationID &&
                                m.TrailerTypeID == trailerType && m.TrailerID == null).Take(1)).ToList()[0];
                                trailer.TrailerID = dispatcherVehicleInfo.TrailerID;
                                trailer.BookingVehicleID = dispatcherVehicleInfo.BookingVehicleID;
                                trailer.IsTrailerAssigned = true;
                                trailer.ModifiedBy = userName;
                                trailer.ModifiedOn = DateTime.Now;

                                DBContext.SaveChanges();
                            }
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void SaveDispatcherInformation(DispatcherVehicleInfoModel dispatcherVehicleInfo)
        {
            using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
            {
                using (var dbContextTransaction = DBContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (dispatcherVehicleInfo.IsVehicleAssigned)
                        {
                            var userName = commonHelper.GetLoggedInUserName();

                            var vehicle = DBContext.BookingVehicleInfoes.SingleOrDefault(v => v.ID == dispatcherVehicleInfo.BookingVehicleID);
                            vehicle.IsJobCompleted = true;
                            vehicle.SpeedoReadingBefore = dispatcherVehicleInfo.SpeedoReadingBeforeLeavingFromDepot;
                            vehicle.SpeedoReadingReturn = dispatcherVehicleInfo.SpeedoReadingReturnToDepot;
                            vehicle.TimeOfLeavingDepot = dispatcherVehicleInfo.TimeOfLeavingDepot;
                            vehicle.TimeOfReturnDepot = dispatcherVehicleInfo.TimeOfReturnDepot;
                            vehicle.ModifiedBy = userName;
                            vehicle.ModifiedOn = DateTime.Now;
                            DBContext.SaveChanges();

                            if (dispatcherVehicleInfo.IsJobCompleted)
                            {
                                var dispatcherVehicleChecklist = DBContext.DispatcherVehicleChecklists.SingleOrDefault(c => c.QuotationID.Equals(dispatcherVehicleInfo.QuotationID) && c.BookingVehicleID.Equals(dispatcherVehicleInfo.BookingVehicleID));
                                dispatcherVehicleChecklist.BookingVehicleID = dispatcherVehicleInfo.BookingVehicleID;
                                dispatcherVehicleChecklist.QuotationID = dispatcherVehicleInfo.QuotationID;
                                dispatcherVehicleChecklist.FullUniform = dispatcherVehicleInfo.FullUniform;
                                dispatcherVehicleChecklist.BriefedPickUpPoint = dispatcherVehicleInfo.BriefedPickUpPoint;
                                dispatcherVehicleChecklist.BriefedFinalDestination = dispatcherVehicleInfo.BriefedFinalDestination;
                                dispatcherVehicleChecklist.Permit = dispatcherVehicleInfo.Permit;
                                dispatcherVehicleChecklist.JobCardAndMaps = dispatcherVehicleInfo.JobCardAndMaps;
                                dispatcherVehicleChecklist.RadioFace = dispatcherVehicleInfo.RadioFace;
                                dispatcherVehicleChecklist.ModifiedBy = userName;
                                dispatcherVehicleChecklist.ModifiedOn = DateTime.Now;

                                DBContext.SaveChanges();

                                var dispatcherVehicleCustomerSurvey = DBContext.DispatcherVehicleCustomerSurveys.SingleOrDefault(c => c.QuotationID.Equals(dispatcherVehicleInfo.QuotationID) && c.BookingVehicleID.Equals(dispatcherVehicleInfo.BookingVehicleID));
                                dispatcherVehicleCustomerSurvey.BookingVehicleID = dispatcherVehicleInfo.BookingVehicleID;
                                dispatcherVehicleCustomerSurvey.QuotationID = dispatcherVehicleInfo.QuotationID;
                                dispatcherVehicleCustomerSurvey.WasDrivingOnTime = dispatcherVehicleInfo.WasDrivingOnTime;
                                dispatcherVehicleCustomerSurvey.WasDriverProperlyAttired = dispatcherVehicleInfo.WasDriverProperlyAttired;
                                dispatcherVehicleCustomerSurvey.DidHeKnowTheRoute = dispatcherVehicleInfo.DidHeKnowTheRoute;
                                dispatcherVehicleCustomerSurvey.DidHeAdhereToTrafficRules = dispatcherVehicleInfo.DidHeAdhereToTrafficRules;
                                dispatcherVehicleCustomerSurvey.AreMapsAvailable = dispatcherVehicleInfo.AreMapsAvailable;
                                dispatcherVehicleCustomerSurvey.CustomerComments = dispatcherVehicleInfo.CustomerComments;
                                dispatcherVehicleCustomerSurvey.ModifiedBy = userName;
                                dispatcherVehicleCustomerSurvey.ModifiedOn = DateTime.Now;

                                DBContext.SaveChanges();
                            }
                            else
                            {
                                var dispatcherVehicleChecklist = new DispatcherVehicleChecklist();
                                dispatcherVehicleChecklist.BookingVehicleID = dispatcherVehicleInfo.BookingVehicleID;
                                dispatcherVehicleChecklist.QuotationID = dispatcherVehicleInfo.QuotationID;
                                dispatcherVehicleChecklist.FullUniform = dispatcherVehicleInfo.FullUniform;
                                dispatcherVehicleChecklist.BriefedPickUpPoint = dispatcherVehicleInfo.BriefedPickUpPoint;
                                dispatcherVehicleChecklist.BriefedFinalDestination = dispatcherVehicleInfo.BriefedFinalDestination;
                                dispatcherVehicleChecklist.Permit = dispatcherVehicleInfo.Permit;
                                dispatcherVehicleChecklist.JobCardAndMaps = dispatcherVehicleInfo.JobCardAndMaps;
                                dispatcherVehicleChecklist.RadioFace = dispatcherVehicleInfo.RadioFace;
                                dispatcherVehicleChecklist.ModifiedBy = userName;
                                dispatcherVehicleChecklist.ModifiedOn = DateTime.Now;

                                DBContext.DispatcherVehicleChecklists.Add(dispatcherVehicleChecklist);
                                DBContext.SaveChanges();

                                var dispatcherVehicleCustomerSurvey = new DispatcherVehicleCustomerSurvey();
                                dispatcherVehicleCustomerSurvey.BookingVehicleID = dispatcherVehicleInfo.BookingVehicleID;
                                dispatcherVehicleCustomerSurvey.QuotationID = dispatcherVehicleInfo.QuotationID;
                                dispatcherVehicleCustomerSurvey.WasDrivingOnTime = dispatcherVehicleInfo.WasDrivingOnTime;
                                dispatcherVehicleCustomerSurvey.WasDriverProperlyAttired = dispatcherVehicleInfo.WasDriverProperlyAttired;
                                dispatcherVehicleCustomerSurvey.DidHeKnowTheRoute = dispatcherVehicleInfo.DidHeKnowTheRoute;
                                dispatcherVehicleCustomerSurvey.DidHeAdhereToTrafficRules = dispatcherVehicleInfo.DidHeAdhereToTrafficRules;
                                dispatcherVehicleCustomerSurvey.AreMapsAvailable = dispatcherVehicleInfo.AreMapsAvailable;
                                dispatcherVehicleCustomerSurvey.CustomerComments = dispatcherVehicleInfo.CustomerComments;
                                dispatcherVehicleCustomerSurvey.ModifiedBy = userName;
                                dispatcherVehicleCustomerSurvey.ModifiedOn = DateTime.Now;

                                DBContext.DispatcherVehicleCustomerSurveys.Add(dispatcherVehicleCustomerSurvey);
                                DBContext.SaveChanges();
                            }

                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public DriverWorkSheetModel GetVehicleDriverSheetInfo(int quotationID, int bookingVehicleID)
        {
            try
            {
                var driverWorkSheetModel = new DriverWorkSheetModel();
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    driverWorkSheetModel = (from quote in DBContext.BookingQuoteInfoes
                                            join booking in DBContext.BookingInfoes on quote.ID equals booking.QuotationID
                                            join vehicle in DBContext.BookingVehicleInfoes on quote.ID equals vehicle.QuotationID
                                            join bus in DBContext.Buses on vehicle.BusID equals bus.ID
                                            join driver in DBContext.Drivers on vehicle.DriverID equals driver.ID
                                            where vehicle.QuotationID.Equals(quotationID) && vehicle.ID.Equals(bookingVehicleID)
                                            select new DriverWorkSheetModel
                                            {
                                                BookingID = booking.ID,
                                                BusNumber = bus.FleetNumber,
                                                CompanyName = quote.CompanyName,
                                                Destination = quote.ToLocation,
                                                PickUpDate = quote.PickUpDate,
                                                PickUpPoint = quote.FromLocation,
                                                PickUpTime = quote.PickUpTime,
                                                ReturnTime = quote.ReturnTime,
                                                DispatcherName = vehicle.ModifiedBy,
                                                DriverNumber = driver.DriverName,
                                                OrganiserName = quote.FirstName,
                                                OrganiserNumber = quote.CellNumber,
                                                ExtraInformation = quote.ExtraInformation
                                            }).SingleOrDefault();

                    var trailer = (from quote in DBContext.BookingQuoteInfoes
                                   join item in DBContext.BookingTrailerInfoes on quote.ID equals item.QuotationID into joined
                                   from i in joined.DefaultIfEmpty()
                                   where i.BookingVehicleID == bookingVehicleID
                                   select i).SingleOrDefault();
                    if (trailer != null && trailer.TrailerID.HasValue)
                    {
                        var trailerNumber = DBContext.Trailers.SingleOrDefault(t => t.ID.Equals(trailer.TrailerID.Value)).FleetNumber;
                        driverWorkSheetModel.TrailerNumber = trailerNumber;
                    }

                }
                return driverWorkSheetModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Driver
        public List<Driver> SearchDrivers(string driverName, string driverNumber)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from driver in DBContext.Drivers
                            where (driver.DriverName.ToLower().Contains(driverName.ToLower()) || driverName == string.Empty) &&
                                    (driver.DriverNumber.ToLower().Contains(driverNumber.ToLower()) || driverNumber == string.Empty)
                            select driver).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveDriverInformation(Driver driver)
        {
            var response = true;
            using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
            {
                using (var dbContextTransaction = DBContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (driver.ID > 0)
                        {
                            response = UpdateDriverInformation(driver, DBContext);
                        }
                        else
                        {
                            response = CreateDriverInformation(driver, DBContext);
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
            return response;
        }

        public bool CreateDriverInformation(Driver driver, SpecialHireDBContext dBContext)
        {
            var response = true;
            var item = dBContext.Drivers.Where(b => b.EmployeeNumber == driver.EmployeeNumber && b.DriverNumber == driver.DriverNumber).Count();
            if (item > 0) { return false; }

            driver.Status = true;
            driver.ModifiedBy = commonHelper.GetLoggedInUserName();
            driver.ModifiedOn = DateTime.Now;

            dBContext.Drivers.Add(driver);
            dBContext.SaveChanges();

            return response;
        }

        public bool UpdateDriverInformation(Driver driver, SpecialHireDBContext dBContext)
        {
            var response = true;
            var i = dBContext.Drivers.Where(b => b.EmployeeNumber == driver.EmployeeNumber && b.DriverNumber == driver.DriverNumber && b.ID != driver.ID).Count();
            if (i > 0) { return false; }

            var item = dBContext.Drivers.SingleOrDefault(m => m.ID == driver.ID);
            item.DriverName = driver.DriverName;
            item.DriverNumber = driver.DriverNumber;
            item.EmployeeNumber = driver.EmployeeNumber;
            item.ContactNumber = driver.ContactNumber;
            item.IsSpecialHireDriver = driver.IsSpecialHireDriver;
            item.Status = driver.Status;
            item.ModifiedBy = commonHelper.GetLoggedInUserName();
            item.ModifiedOn = DateTime.Now;

            dBContext.SaveChanges();

            return response;
        }

        public bool CheckDriverDependency(int driverID)
        {
            var today = DateTime.Now.Date;
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from vehicle in DBContext.BookingVehicleInfoes
                            join quote in DBContext.BookingQuoteInfoes on vehicle.QuotationID equals quote.ID
                            where (quote.PickUpDate >= today || quote.ReturnDate >= today)
                            && (vehicle.DriverID.HasValue && vehicle.DriverID.Value.Equals(driverID))
                            select vehicle).ToList().Count() > 0 ? true : false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Availability

        public List<BookingAvailabilityInfoModel> GetAvailableBookingSummary(double start, double end)
        {
            var fromDate = commonHelper.ConvertFromUnixTimestamp(start);
            var toDate = commonHelper.ConvertFromUnixTimestamp(end);
            using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
            {
                var rslt = DBContext.BookingAvailabilityInfoes.Where(s => s.PickUpDate >= fromDate && s.PickUpDate <= toDate);
                var holiday = GetHolidayList(start, end);

                List<BookingAvailabilityInfoModel> result = new List<BookingAvailabilityInfoModel>();
                foreach (var item in rslt)
                {
                    BookingAvailabilityInfoModel rec = new BookingAvailabilityInfoModel();
                    rec.ID = "No Of Busses:" + item.NoOfBusses.ToString() + " <br/> No Of Trailers:" + item.NoOfTrailers.ToString() + " <br/> No Of Busses Confirmed:" + item.NoOfBussesConfirmed.ToString() + " <br/> No Of Trailers Confirmed:" + item.NoOfTrailersConfirmed.ToString();
                    rec.StartDateString = item.PickUpDate.ToString("s");
                    rec.EndDateString = item.ReturnDate.ToString("s");
                    rec.Title = "No Of Booking:" + item.NoOfBooking.ToString();
                    rec.Color = "#5bc0de";
                    result.Add(rec);
                }

                foreach (var item in holiday)
                {
                    BookingAvailabilityInfoModel rec = new BookingAvailabilityInfoModel();
                    rec.ID = item.HolidayDescription + " </br> " + item.HolidayDate.ToString("dd-MMMM-yyyy");
                    rec.StartDateString = item.HolidayDate.ToString("s");
                    rec.EndDateString = item.HolidayDate.ToString("s");
                    rec.Title = item.HolidayDescription;
                    rec.Color = "#FF9933";
                    result.Add(rec);
                }

                return result;
            }
        }

        public List<PublicHoliday> GetHolidayList(double start, double end)
        {
            var fromDate = commonHelper.ConvertFromUnixTimestamp(start);
            var toDate = commonHelper.ConvertFromUnixTimestamp(end);
            using (EBusAdministrationContext DBContext = new EBusAdministrationContext())
            {
                var result = DBContext.PublicHolidays.Where(s => s.HolidayDate >= fromDate && s.HolidayDate <= toDate);
                return result.ToList();
            }
        }

        public List<BookingAvailabilityInfoModel> GetAvailableBooking(double start, double end)
        {
            return new List<BookingAvailabilityInfoModel>();
        }

        #endregion

        #region Reports
        public List<BookingReporting> SearchBookingsForTrackBookings(string organiserName, string companyName, string invoiceNumber, string driverName,
            string driverNumber, string fleetNumber, string quotationNumber, string dateFrom, string dateTo)
        {
            var resp = new List<BookingQuoteInfo>();
            var fromDate = new DateTime();
            var toDate = new DateTime();
            if (dateFrom != string.Empty)
            {
                fromDate = Convert.ToDateTime(dateFrom);
            }
            if (dateTo != string.Empty)
            {
                toDate = Convert.ToDateTime(dateTo);
            }

            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var query = (from record in DBContext.BookingReportings
                                 where (record.QuotationNumber.ToLower().Equals(quotationNumber.ToLower()) || quotationNumber == string.Empty) &&
                                         (record.OrganiserName.ToLower().Contains(organiserName.ToLower()) || organiserName == string.Empty) &&
                                         (record.CompanyName.ToLower().Contains(companyName.ToLower()) || companyName == string.Empty) &&
                                         (record.InvoiceNumber.ToLower().Equals(invoiceNumber.ToLower()) || invoiceNumber == string.Empty) &&
                                         (record.DriverName.ToLower().Contains(driverName.ToLower()) || driverName == string.Empty) &&
                                         (record.DriverNumber.ToLower().Contains(driverNumber.ToLower()) || driverNumber == string.Empty) &&
                                         (record.FleetNumber.ToLower().ToLower().Equals(fleetNumber.ToLower()) || fleetNumber == string.Empty)
                                 select record);

                    if (dateFrom != string.Empty && dateTo != string.Empty)
                    { query = query.Where(q => DbFunctions.TruncateTime(q.InvoiceDate) >= fromDate.Date && DbFunctions.TruncateTime(q.InvoiceDate) <= toDate.Date); }
                    else if (dateFrom != string.Empty) { query = query.Where(q => DbFunctions.TruncateTime(q.InvoiceDate) == fromDate.Date); }
                    else if (dateTo != string.Empty) { query = query.Where(q => DbFunctions.TruncateTime(q.InvoiceDate) == toDate.Date); }

                    return query.OrderBy(q=>q.InvoiceDate).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<BookingReporting> SearchBookingsForQuotations(string ReportType, string dateFrom, string dateTo)
        {
            var resp = new List<BookingQuoteInfo>();
            var fromDate = new DateTime();
            var toDate = new DateTime();
            if (dateFrom != string.Empty)
            {
                fromDate = Convert.ToDateTime(dateFrom);
            }
            if (dateTo != string.Empty)
            {
                toDate = Convert.ToDateTime(dateTo);
            }

            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var query = (from record in DBContext.BookingReportings select record);
                    if (dateFrom != string.Empty && dateTo != string.Empty)
                    { query = query.Where(q => DbFunctions.TruncateTime(q.QuotationDate) >= fromDate.Date && DbFunctions.TruncateTime(q.QuotationDate) <= toDate.Date); }
                    else if (dateFrom != string.Empty) { query = query.Where(q => DbFunctions.TruncateTime(q.QuotationDate) == fromDate.Date); }
                    else if (dateTo != string.Empty) { query = query.Where(q => DbFunctions.TruncateTime(q.QuotationDate) == toDate.Date); }

                    switch (ReportType)
                    {
                        case "1":
                            //gets all Quotations
                            break;
                        case "2":
                            query = query.Where(q => q.InvoiceNumber.Trim() != string.Empty);
                            break;
                        case "3":
                            query = query.Where(q => q.InvoiceNumber.Trim() == string.Empty);
                            break;
                        default:
                            break;
                    }
                    return query.OrderBy(q => q.QuotationDate).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<BookingReporting> SearchBookingsForInvoices(string dateFrom, string dateTo)
        {
            var resp = new List<BookingQuoteInfo>();
            var fromDate = new DateTime();
            var toDate = new DateTime();
            if (dateFrom != string.Empty)
            {
                fromDate = Convert.ToDateTime(dateFrom);
            }
            if (dateTo != string.Empty)
            {
                toDate = Convert.ToDateTime(dateTo);
            }

            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var query = (from record in DBContext.BookingReportings
                                 where record.InvoiceNumber.Trim() != string.Empty
                                 select record);
                    if (dateFrom != string.Empty && dateTo != string.Empty)
                    { query = query.Where(q => DbFunctions.TruncateTime(q.InvoiceDate) >= fromDate.Date && DbFunctions.TruncateTime(q.InvoiceDate) <= toDate.Date); }
                    else if (dateFrom != string.Empty) { query = query.Where(q => DbFunctions.TruncateTime(q.InvoiceDate) == fromDate.Date); }
                    else if (dateTo != string.Empty) { query = query.Where(q => DbFunctions.TruncateTime(q.InvoiceDate) == toDate.Date); }

                    return query.OrderBy(q => q.InvoiceDate).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public object GetMonthlyRevenue(string year)
        {
            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    return (from item in DBContext.Revenues.AsEnumerable()
                            where (year.Trim() == string.Empty || item.OrderYear.Trim() == year.Trim())
                            select new
                            {
                                Month = item.OrderMonth,
                                Revenue = item.Total
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<BookingReporting> SearchBookingsForScheduledWorked(string organiserName, string companyName, string invoiceNumber, string driverName,
    string driverNumber, string fleetNumber, string quotationNumber, string dateFrom, string dateTo)
        {
            var resp = new List<BookingQuoteInfo>();
            var fromDate = new DateTime();
            var toDate = new DateTime();
            if (dateFrom != string.Empty)
            {
                fromDate = Convert.ToDateTime(dateFrom);
            }
            if (dateTo != string.Empty)
            {
                toDate = Convert.ToDateTime(dateTo);
            }

            try
            {
                using (SpecialHireDBContext DBContext = new SpecialHireDBContext(ConfigurationSettings.ConnectionKey))
                {
                    var query = (from record in DBContext.BookingReportings
                                 where (record.QuotationNumber.ToLower().Equals(quotationNumber.ToLower()) || quotationNumber == string.Empty) &&
                                         (record.OrganiserName.ToLower().Contains(organiserName.ToLower()) || organiserName == string.Empty) &&
                                         (record.CompanyName.ToLower().Contains(companyName.ToLower()) || companyName == string.Empty) &&
                                         (record.InvoiceNumber.ToLower().Equals(invoiceNumber.ToLower()) || invoiceNumber == string.Empty) &&
                                         (record.DriverName.ToLower().Contains(driverName.ToLower()) || driverName == string.Empty) &&
                                         (record.DriverNumber.ToLower().Contains(driverNumber.ToLower()) || driverNumber == string.Empty) &&
                                         (record.FleetNumber.ToLower().ToLower().Equals(fleetNumber.ToLower()) || fleetNumber == string.Empty) //&&
                                         //record.IsJobCompleted == true
                                 select record);

                    if (dateFrom != string.Empty && dateTo != string.Empty)
                    { query = query.Where(q => DbFunctions.TruncateTime(q.InvoiceDate) >= fromDate.Date && DbFunctions.TruncateTime(q.InvoiceDate) <= toDate.Date); }
                    else if (dateFrom != string.Empty) { query = query.Where(q => DbFunctions.TruncateTime(q.InvoiceDate) == fromDate.Date); }
                    else if (dateTo != string.Empty) { query = query.Where(q => DbFunctions.TruncateTime(q.InvoiceDate) == toDate.Date); }

                    return query.OrderBy(q => q.InvoiceDate).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}