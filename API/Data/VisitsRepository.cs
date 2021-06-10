using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data {
    public class VisitsRepository : IVisitsRepository {
        private readonly DataContext _context;
        public VisitsRepository (DataContext context) {
            _context = context;

        }

        public async Task<UserVisits> GetUserVisit(int sourceUserId, int visitsUserId)
        {
            return await _context.Visits.FindAsync(sourceUserId, visitsUserId);

        }

        public async Task<PagedList<VisitsDto>> GetUserVisits(VisitsParams visitsParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var visits = _context.Visits.AsQueryable();

            if (visitsParams.Predicate == "visited") {
                visits = visits.Where(visit => visit.SourceUserId == visitsParams.UserId);
                users = visits.Select(visit => visit.VisitedUser); 
                // List of users visit by the current user
            }

            if (visitsParams.Predicate == "all")
            {
                IQueryable<AppUser> UsersIvisited = GetAppUsersIvisited(visitsParams);
                IQueryable<AppUser> UsersVisitedme = GetAppUsersVisitedme(visitsParams);               
                IQueryable<AppUser> AllUsers = UsersIvisited .Concat(UsersVisitedme);
                users = AllUsers;
                // List of all users visit
            }

            if (visitsParams.Predicate == "pastMonth")
            {
                IQueryable<AppUser> UsersIvisitedByMonth = GetAppUsersIVisitedByMonth(visitsParams);
                IQueryable<AppUser> UsersVisitedMeByMonth = GetAppUsersVisitedMeByMonth (visitsParams);               
                IQueryable<AppUser> AllUsersByMonth = UsersIvisitedByMonth.Concat(UsersVisitedMeByMonth);
                users = AllUsersByMonth;
                // List of all users visit
            }


            if (visitsParams.Predicate == "visitedBy") {
                visits = visits.Where (visit => visit.VisitedUserId == visitsParams.UserId);
                users = visits.Select (visit => visit.SourceUser); 
                // List of users who have visited the current user
            }

            var visitedUsers =  users.Select(user => new VisitsDto {
                    Username = user.UserName,
                    KnownAs = user.KnownAs,
                    Age = user.DateOfBirth.CalculateAge(),
                    PhotoUrl = user.Photos.FirstOrDefault (p => p.IsMain).Url,
                    City = user.City,
                    Id = user.Id
            });

            return await PagedList<VisitsDto>.CreateAsync(visitedUsers,
            visitsParams.PageNumber, visitsParams.PageSize); 
        }

        //get list of users who visited me
        private IQueryable<AppUser> GetAppUsersVisitedme(VisitsParams v)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var visits = _context.Visits.AsQueryable();

            visits = visits.Where(visit => visit.VisitedUserId != v.UserId);
            return users = visits.Select(visit => visit.VisitedUser);
        }

        // get list of users who I visited
        private IQueryable<AppUser> GetAppUsersIvisited(VisitsParams v)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var visits = _context.Visits.AsQueryable();

            visits = visits.Where(visit => visit.SourceUserId != v.UserId);
            return users= visits.Select(visit => visit.SourceUser);
        }


        //get list of users who visited me in the past month
        private IQueryable<AppUser> GetAppUsersVisitedMeByMonth(VisitsParams v)
        {
            var pastMonth = DateTime.UtcNow.AddMonths(-1);
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var visits = _context.Visits.AsQueryable();

            visits = visits.
                    Where(visit => visit.VisitedUserId != v.UserId &&
                    visit.Created >= pastMonth);
                    
            return users = visits.Select(visit => visit.VisitedUser);
        }

        // get list of users who I visited in the past month
        private IQueryable<AppUser> GetAppUsersIVisitedByMonth(VisitsParams v)
        {
            var pastMonth = DateTime.UtcNow.AddMonths(-1);
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var visits = _context.Visits.AsQueryable();

            visits = visits.
            Where(visit => visit.SourceUserId != v.UserId
            && visit.Created >= pastMonth);

            return users= visits.Select(visit => visit.SourceUser);
        }


        public async Task<AppUser> GetUserWithVisits(int userId)
        {
            return await _context.Users
                .Include (x => x.VisitedUsers)
                .FirstOrDefaultAsync (x => x.Id == userId);
        }
    }
}