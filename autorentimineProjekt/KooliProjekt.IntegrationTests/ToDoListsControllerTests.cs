//using System.Net;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using KooliProjekt.Application.Data;
//using KooliProjekt.Application.Infrastructure.Paging;
//using KooliProjekt.Application.Infrastructure.Results;
//using KooliProjekt.IntegrationTests.Helpers;
//using Xunit;

//namespace KooliProjekt.IntegrationTests
//{
//    [Collection("Sequential")]
//    public class TodoListsControllerTests : TestBase
//    {
//        [Fact]
//        public async Task List_should_return_paged_result()
//        {
//            // Arrange
//            var url = "/api/ToDoLists/List/?page=0&pageSize=0";

//            // Act
//            var response = await Client.GetFromJsonAsync<OperationResult<PagedResult<ToDoList>>>(url);

//            // Assert
//            Assert.NotNull(response);
//            Assert.False(response.HasErrors);
//        }

//        [Fact]
//        public async Task Get_should_return_list()
//        {
//            // Arrange
//            var url = "/api/ToDoLists/Get/?id=1";
            
//            var todoList = new ToDoList { Title = "Test List" };
//            await DbContext.AddAsync(todoList);
//            await DbContext.SaveChangesAsync();

//            // Act
//            var response = await Client.GetFromJsonAsync<OperationResult<ToDoList>>(url);
            
//            // Assert
//            Assert.NotNull(response);
//            Assert.False(response.HasErrors);
//        }

//        [Fact]
//        public async Task Get_should_return_not_found_for_missing_list()
//        {
//            // Arrange
//            var url = "/api/ToDoLists/Get/?id=131";

//            // Act
//            var response = await Client.GetAsync(url);

//            // Assert
//            Assert.NotNull(response);
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//        }
//    }
//}