using System.Collections.Generic;

public interface IGachaService
{
    GachaResult Roll();
    List<GachaResult> RollTen();
}
