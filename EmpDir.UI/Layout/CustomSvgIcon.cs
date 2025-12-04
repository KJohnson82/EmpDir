using Telerik.SvgIcons;

//namespace MMDirectory.Layout
//{
public class CustomSvgIcon : ISvgIcon
{
    public string Name { get; set; }
    public string Content { get; set; }
    public string ViewBox { get; set; } = "0 -960 960 960";


    public CustomSvgIcon(string name, string content, string viewbox)
    {
        Name = name;
        Content = content;
        ViewBox = viewbox;

    }
}

public class CustomIcons
{
    public static ISvgIcon Home => new CustomSvgIcon(
        "home",
        "<path d=\"M4 21V9l8-6l8 6v12h-6v-7h-4v7z\"/>",
        "0 0 24 24"
    );

    public static ISvgIcon Corporate => new CustomSvgIcon(
        "corporate",
        "<path d=\"M2 21V3h10v4h10v14zm2-2h6v-2H4zm0-4h6v-2H4zm0-4h6V9H4zm0-4h6V5H4zm8 12h8V9h-8zm2-6v-2h4v2zm0 4v-2h4v2z\" />",
        "0 0 24 24"
    );

    public static ISvgIcon MetalMart => new CustomSvgIcon(
        "store",
        "<path d=\"M4 6V4h16v2zm0 14v-6H3v-2l1-5h16l1 5v2h-1v6h-2v-6h-4v6zm2-2h6v-4H6zm-.95-6h13.9zm0 0h13.9l-.6-3H5.65z\"/>",
        "0 0 24 24"
    );

    public static ISvgIcon ServiceCenter => new CustomSvgIcon(
        "service_center",
        "<path d=\"m12 3l8 6v12H4V9zm0 12q1.25 0 2.125-.875T15 12q0-1.25-.875-2.125T12 9q-1.25 0-2.125.875T9 12q0 1.25.875 2.125T12 15m0-2q-.425 0-.712-.288T11 12q0-.425.288-.712T12 11q.425 0 .713.288T13 12q0 .425-.288.713T12 13m0 5q-1.025 0-2 .25T8.15 19h7.7q-.875-.5-1.85-.75T12 18m-6-8v8q1.3-.975 2.825-1.487T12 16q1.65 0 3.175.513T18 18v-8l-6-4.5zm6 2\"/>",
        "0 0 24 24"
    );

    public static ISvgIcon Plant => new CustomSvgIcon(
        "plant",
        "<path d=\"M2 22V9.975L9 7v2l5-2v3h8v12zm2-2h16v-8h-8V9.95l-5 2V10l-3 1.325zm7-2h2v-4h-2zm-4 0h2v-4H7zm8 0h2v-4h-2zm7-8h-5l1-8h3zM4 20h16z\"/>",
        "0 0 24 24"
    );

    public static ISvgIcon LeftArrow => new CustomSvgIcon(
        "chevron-left",
        "<path d=\"M9.4 233.4c-12.5 12.5-12.5 32.8 0 45.3l192 192c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L77.3 256 246.6 86.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0l-192 192z\" />",
        "0 0 320 512"
    );

    public static ISvgIcon Star => new CustomSvgIcon(
        "star",
        "<path d=\"m233-120 65-281L80-590l288-25 112-265 112 265 288 25-218 189 65 281-247-149-247 149Z\"/>",
        "0 -960 960 960"
    );

    public static ISvgIcon Hand_Gesture => new CustomSvgIcon(
        "hand_gesture",
        "<path d=\"M240-40q-83 0-141.5-58.5T40-240h60q0 58 41 99t99 41v60Zm162 0q-30 0-56-13.5T303-92L48-465l24-23q19-19 45-22t47 12l116 81v-383q0-17 11.5-28.5T320-840q17 0 28.5 11.5T360-800v320h80v-400q0-17 11.5-28.5T480-920q17 0 28.5 11.5T520-880v400h80v-360q0-17 11.5-28.5T640-880q17 0 28.5 11.5T680-840v360h80v-280q0-17 11.5-28.5T800-800q17 0 28.5 11.5T840-760v560q0 66-47 113T680-40H402Zm478-719q0-51-35-86t-86-35v-60q75 0 128 53t53 128h-60Z\"/>",
        "0 -960 960 960"
    );

    public static ISvgIcon Waving_Hand => new CustomSvgIcon(
        "waving_hand",
        "<path d=\"M39-680q0-100 70.5-170.5T280-921v81q-66 0-113 47t-47 113H39Zm173 469q-91-91-91-219t91-219l70-71 12 12q29 29 29 70.5T294-567l-14 14q-12 12-12 28.5t12 28.5l36 36q26 26 26 63t-26 63l43 43q44-44 44-105.5T358-503l-22-22q26-26 37-58.5t9-66.5l179-179q12-12 28.5-12t28.5 12q12 12 12 28.5T618-772L431-585l42 42 241-240q12-12 28-12t28 12q12 12 12 28t-12 28L530-486l42 42 212-212q12-12 28.5-12t28.5 12q12 12 12 28.5T841-599L629-387l42 42 162-162q12-12 28.5-12t28.5 12q12 12 12 28.5T890-450L650-211q-91 91-219 91t-219-91ZM680-39v-81q66 0 113-47t47-113h81q0 100-70.5 170.5T680-39Z\"/>",
        "0 -960 960 960"
    );

    public static ISvgIcon Face => new CustomSvgIcon(
        "face",
        "<path d=\"M360-390q-21 0-35.5-14.5T310-440q0-21 14.5-35.5T360-490q21 0 35.5 14.5T410-440q0 21-14.5 35.5T360-390Zm240 0q-21 0-35.5-14.5T550-440q0-21 14.5-35.5T600-490q21 0 35.5 14.5T650-440q0 21-14.5 35.5T600-390ZM480-160q134 0 227-93t93-227q0-24-3-46.5T786-570q-21 5-42 7.5t-44 2.5q-91 0-172-39T390-708q-32 78-91.5 135.5T160-486v6q0 134 93 227t227 93Zm0 80q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-83 31.5-156T197-763q54-54 127-85.5T480-880q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 83-31.5 156T763-197q-54 54-127 85.5T480-80Zm-54-715q42 70 114 112.5T700-640q14 0 27-1.5t27-3.5q-42-70-114-112.5T480-800q-14 0-27 1.5t-27 3.5ZM177-581q51-29 89-75t57-103q-51 29-89 75t-57 103Zm249-214Zm-103 36Z\"/>",
        "0 -960 960 960"
    );

    public static ISvgIcon Factory => new CustomSvgIcon(
        "factory",
        "<path d=\"M80-80v-481l280-119v80l200-80v120h320v480H80Zm80-80h640v-320H480v-82l-200 80v-78l-120 53v347Zm280-80h80v-160h-80v160Zm-160 0h80v-160h-80v160Zm320 0h80v-160h-80v160Zm280-320H680l40-320h120l40 320ZM160-160h640-640Z\"/>",
        "0 -960 960 960"
    );

    public static ISvgIcon Domain => new CustomSvgIcon(
        "domain",
        "<path d=\"M80-120v-720h400v160h400v560H80Zm80-80h80v-80h-80v80Zm0-160h80v-80h-80v80Zm0-160h80v-80h-80v80Zm0-160h80v-80h-80v80Zm160 480h80v-80h-80v80Zm0-160h80v-80h-80v80Zm0-160h80v-80h-80v80Zm0-160h80v-80h-80v80Zm160 480h320v-400H480v80h80v80h-80v80h80v80h-80v80Zm160-240v-80h80v80h-80Zm0 160v-80h80v80h-80Z\"/>",
        "0 -960 960 960"
    );

    public static ISvgIcon Emoji_People => new CustomSvgIcon(
        "emoji_people",
        "<path d=\"M360-80v-529q-91-24-145.5-100.5T160-880h80q0 83 53.5 141.5T430-680h100q30 0 56 11t47 32l181 181-56 56-158-158v478h-80v-240h-80v240h-80Zm120-640q-33 0-56.5-23.5T400-800q0-33 23.5-56.5T480-880q33 0 56.5 23.5T560-800q0 33-23.5 56.5T480-720Z\"/>",
        "0 -960 960 960"
    );

}
//}
