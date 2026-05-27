namespace API.Services.Background;

public interface IDailyShowingsGeneratorService {
	Task GenerateDailyShowingsAsync(CancellationToken cancellationToken = default);
}