# SQLitePragmaPerf
**IN EARLY DEVELOPMENT. PLEASE IGNORE FOR NOW**

*Wer mißt, mißt Mist* -German proverb

This project tests the effect of System.Data.SQLite database connection settings or [PRAGMA][pragmas] statement on different SQL statements. When all tests are run, it generates CSV files which can be further analyzed.

It’s intent to proof or disproof if pragma X set to Y would result in a 420% performance gain. 

##<a name="limitations">Limitations</a>

This project depends on the [StopWatch][_stopwatch] class, it cannot provide 100% [accurate timings][no_acc_timing] and is **not** a benchmark tool.

It does also not support all SQLite PRAGMA statements, for example [checkpoint_fullfsync](http://www.sqlite.org/pragma.html#pragma_checkpoint_fullfsync) or [fullfsync](http://www.sqlite.org/pragma.html#pragma_fullfsync) as those are only supported on Mac OS. 

##<a name="download">Download</a>

The most recent version can be downloaded from [Releases][_downloads].

##<a name="docs">Documentation</a>

Please see the [Wiki][_wiki] for documentation. 

##<a name="docs">Additional resources</a>

* [DB Browser for SQLite](http://sqlitebrowser.org/) 
* [Test Data: Lahman’s Baseball Database](http://seanlahman.com/baseball-archive/statistics/)
* [SQLite, RowID, INDEX and AUTOINCREMENT](http://texhex.blogspot.com/2016/01/sqlite-rowid-index-and-autoincrement.html)

##<a name="contribute">Contributions</a>

Any constructive contribution is very welcome! 

If you encounter a bug, please create a new [issue][_issuenew], describing how to reproduce the bug and we will try to fix it. Pull requests for source code changes or scripts are welcome as well.  


##<a name="copyright">Copyright and License</a>

Copyright © 2016 [Michael ‘Tex’ Hex][_texhexhomepage] ([@texhex][_texhexgithub]).

Licensed under the **Apache License, Version 2.0**. For details, please see [LICENSE.txt][_license].






[_issuenew]:https://github.com/texhex/SQLitePragmaPerf/issues/new
[_wiki]: https://github.com/texhex/SQLitePragmaPerf/wiki
[_downloads]: https://github.com/texhex/SQLitePragmaPerf/releases
[_license]: https://github.com/texhex//SQLitePragmaPerf/blob/master/licenses/LICENSE.txt

[_texhexgithub]:https://github.com/texhex/
[_texhexhomepage]:http://www.texhex.info/


[_stopwatch]: https://msdn.microsoft.com/en-us/library/system.diagnostics.stopwatch%28v=vs.110%29.aspx
[pragmas]: https://www.sqlite.org/pragma.html
[no_acc_timing]: http://stackoverflow.com/a/14019738/612954

