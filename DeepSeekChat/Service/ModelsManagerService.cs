using DeepSeekChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Service;

[JsonStorageFile(FileName = "models.json")]
public class ModelsManagerService : JsonSeriailizingServiceBase<AiModelStorage>
{
    public ModelsManagerService() : base()
    {

    }

    public List<AiModel> GetStroragedModels()
    {
        return _data.AiModels;
    }

    public AiModel CreateNewModel(string name, string description, string model)
    {
        var aiModel = new AiModel()
        {
            Name = name,
            Description = description,
            ModelID = model,
            UniqueID = Guid.NewGuid(),
        };
        _data.AiModels.Add(aiModel);
        return aiModel;
    }

    public void RemoveModel(Guid id)
    {
        var aiModel = _data.AiModels.FirstOrDefault(x => x.UniqueID == id);
        if (aiModel != null)
        {
            _data.AiModels.Remove(aiModel);
        }
    }

    public AiModel GetModelById(Guid id)
    {
        return _data.AiModels.FirstOrDefault(x => x.UniqueID == id);
    }

    public void SelectModel(Guid id)
    {
        var aiModel = _data.AiModels.FirstOrDefault(x => x.UniqueID == id);
        if (aiModel != null)
        {
            _data.SelectedModel = aiModel.UniqueID;
        }
    }
}
