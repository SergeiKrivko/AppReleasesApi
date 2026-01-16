using CommandLine;

namespace AppReleases.Cli;

public class Arguments
{
    [Option(shortName: 'd', longName: "directory", Required = false,
        HelpText = "Директория, из которой следует взять ассеты. Если не указана, ассеты не будут загружены")]
    public string? Directory { get; set; }

    [Option(shortName: 'b', longName: "bundle", Required = false, HelpText = "Список файлов установщиков")]
    public IEnumerable<string> Bundles { get; set; } = [];

    [Option(shortName: 'u', longName: "url", Required = true, HelpText = "Ссылка на API")]
    public string Url { get; set; } = null!;

    [Option(shortName: 'v', longName: "version", Required = true, HelpText = "Версия создаваемого релиза")]
    public string Version { get; set; } = null!;

    [Option(shortName: 'p', longName: "platform", Required = false,
        HelpText = "Платформа, для которой предназначен релиз. По умолчанию релиз будет считаться кроссплатформенным")]
    public string? Platform { get; set; }

    [Option('b', "branch", Required = false,
        HelpText =
            "Название ветки. Если ветки не существует, она будет создана с базовыми настройками. По умолчанию будет использована основная ветка приложения")]
    public string? Branch { get; set; }

    [Option('a', "application", Required = true, HelpText = "Идентификатор приложения")]
    public string Application { get; set; } = null!;

    [Option('t', "token", Required = true, HelpText = "Токен авторизации")]
    public string Token { get; set; } = null!;

    [Option(longName: "from-root", Required = false,
        HelpText = "Директория, из которой следует взять ассеты. " +
                   "При этом относительный путь от этой директории будет рассматриваться как абсолютный для установленного файла. " +
                   "Может быть передан вместе с --directory.")]
    public string? FromRootDirectory { get; set; }
}