Animu-Torrent-Checker
=====================

Simple program to download anime torrent automatically. Current version is console based, using .NET framework to fetch torrent data from Nyaa.

## Usage

Compile, business as usual. 

There are two XML files that must be configured, Settings and AnimuSeries:

1. Place Settings.xml on the same folder as the executable. Change the AnimeListPath (where you will put AnimuSeries.xml) and TorrentDownloadFolder (folder where you want to put downloaded torrent files, this folder must exists!) to where you want it to be.
2. Place AnimuSeries.xml to the path you set up in Settings. Add/remove anime information to what you want to automaticallt download.

Anime information itself are represented as xml data, you can see the usage in AnimeSeries.xml. Change the title, subber, and episode to the one where you want to start. Copy and change again if you want to check another series.
```
<Animu>
  <Title>Chuunibyou demo Koi ga Shitai! Ren</Title>
  <Episode>4</Episode>
  <Subber>HorribleSubs</Subber>
</Animu>
```

## Further Usage

This program is designed and can be used from far away by integrating it with cloud-backing-software and torrent downloaded program. My normal use case are:

1. Put AnimuSeries.xml and Torrent folder on Dropbox folder.
2. Refer that path in Settings.xml and executable on the local machine (machine where you want to download stuffs, ex: home PC)
3. Refer the TorrentDownloadFolder path on uTorrent or something to automatically run the torrent on the folder.
4. Run the program and uTorrent in that local machine.

Using the program like this, you can simply update the series from the cloud. You can also see which torrent that already downloaded and being ran (extension changed to .loaded in case of uTorrent) from wherever. When you get home, hopefully all the running torrent download will already finished and you can just watch it.

## Roadmap

Below are list of feature that I want to add/change in the future:

1. Bug fixing / error checking.
2. Change implementation to support Mono to enable cross platform. Or, rewrite in Go.
3. Support last episode vs delete series manually from XML.
4. Fetch list of ongoing anime and it's data automatically (ex: from anidb). User simply select the series they want to follow.
5. User Interface for public distribution.
