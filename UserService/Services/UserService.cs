using AutoMapper;
using UserService.Dtos;
using UserService.Entities;
using UserService.Repositories;

namespace UserService.Service
{
    public class UserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepo userRepo, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public UserReadDto CreateUser(UserCreateDto userCreateDto)
        {

            var user = _mapper.Map<User>(userCreateDto);
            _userRepo.AddUser(ref user);
            
            return _mapper.Map<UserReadDto>(user);
        }

        public UserReadDto? GetUserByUsername(string username)
        {
            var user = _userRepo.GetUserByUsername(username);
            if (user == null)
            {
                _logger.LogWarning($"User with Username {username} was not found.");
                return null;
            }

            return _mapper.Map<UserReadDto>(user);
        }

        public UserReadDto? GetUserById(int userId)
        {
            var user = _userRepo.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} was not found.");
                return null;
            }

            return _mapper.Map<UserReadDto>(user);
        }

        public IEnumerable<UserReadDto> GetAllUsers()
        {
            var users = _userRepo.GetAllUsers();
            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public void UpdateUser(int userId, UserCreateDto userUpdateDto)
        {
            var user = _userRepo.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} was not found during update.");
                return;
            }

            _mapper.Map(userUpdateDto, user);
            _userRepo.UpdateUser(user);
        }

        public void DeleteUser(int userId)
        {
            var user = _userRepo.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} was not found during delete.");
                return;
            }
            _userRepo.DeleteUser(user);
        }
    }
}