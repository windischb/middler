
declare namespace System {

    export class Uri {
        AbsolutePath: string;
        AbsoluteUri: string;
        Authority: string;
        DnsSafeHost: string;
        Fragment: string;
        Host: string;
        HostNameType: string;
        IsAbsoluteUri: boolean;
        IsDefaultPort: boolean;
        IsFile: boolean;
        IsLoopback: boolean;
        IsUnc: boolean;
        LocalPath: string;
        OriginalString: string;
        PathAndQuery: string;
        Port: number;
        Query: string;
        Scheme: string;
        Segments: Array<string>;
        UserEscaped: boolean;
        UserInfo: string;
    }

}