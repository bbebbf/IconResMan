namespace IconResMan
{
    public class ConsoleWriter : IResourceWriter
    {
        public void WriteGroupIcon(GroupIcon groupIcon)
        {
            Console.Write($"Group icon {groupIcon.KeyLang.Key.ResourceName}, language = {groupIcon.KeyLang.Language}, icon count = {groupIcon.Record.wCount} \n");
        }
    }
}
