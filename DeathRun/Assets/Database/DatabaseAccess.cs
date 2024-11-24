using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DatabaseAccess : MonoBehaviour
{
    public class UserInform
    {
        public string Id { get; set; }
        public string Password { get; set; }
    }

    MongoClient client = new MongoClient("mongodb+srv://testUser:testPassword@cluster0.2pzu3.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
    IMongoDatabase database;
    IMongoCollection<BsonDocument> collection;

    void Start()
    {
        database = client.GetDatabase("LoginDB");
        collection = database.GetCollection<BsonDocument>("LoginCollection");
    }

    public async Task<Define.AccountCreationStatus> SaveNewAccountToDataBase(string id, string password, string nickname)
    {
        List<UserInform> userInforms = new List<UserInform>();

        FilterDefinition<BsonDocument> filterById = Builders<BsonDocument>.Filter.Eq("Id", id);
        List<BsonDocument> resultsById = await collection.Find(filterById).ToListAsync();

        if (resultsById.Count != 0)
        {
            return Define.AccountCreationStatus.DuplicateId;
        }

        FilterDefinition<BsonDocument> filterBynickName = Builders<BsonDocument>.Filter.Eq("Nickname", nickname);
        List<BsonDocument> resultsBynickName = await collection.Find(filterBynickName).ToListAsync();

        if (resultsBynickName.Count != 0)
        {
            return Define.AccountCreationStatus.DuplicateNickname;
        }

        var document = new BsonDocument() {
            { "Id", id },
            {"Password", password },
            {"Nickname", nickname }
        };
        await collection.InsertOneAsync(document);
        //컬렉션 갱신
        collection = database.GetCollection<BsonDocument>("LoginCollection");

        return Define.AccountCreationStatus.Success;
    }

    public async Task<(Define.LoginStatus, string)> CheckUserInformsFromDataBase(string id, string password)
    {
        List<UserInform> userInforms = new List<UserInform>();

        FilterDefinition<BsonDocument> filterById = Builders<BsonDocument>.Filter.Eq("Id", id);
        List<BsonDocument> resultsById = await collection.Find(filterById).ToListAsync();

        if (resultsById.Count == 0)
        {
            return (Define.LoginStatus.IDNotFound, null);
        }

        foreach (BsonDocument result in resultsById)
        {
            UserInform userInform = new UserInform();
            if (password == result.GetElement("Password").Value.ToString())
            {
                string nickname = result.GetElement("Nickname").Value.ToString();
                return (Define.LoginStatus.Success, nickname);
            }
        }

        return (Define.LoginStatus.PasswordNotFound, null);
    }

    // Update is called once per frame
    void Update()
    {

    }
}