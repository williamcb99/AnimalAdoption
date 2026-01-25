import { useState, useEffect } from "react";

type AnimalVideoProps = {
    index?: number;
    isRandom?: boolean;
}

type VideoData = {
    sourceUrl: string;
    posterUrl: string;
};

const VIDEO_DATA: VideoData[] = [
    {
        sourceUrl: "https://assets.mixkit.co/videos/1535/1535-1080.mp4",
        posterUrl: "https://assets.mixkit.co/videos/1535/1535-thumb-1080-1.jpg"
    },
    {
        sourceUrl: "https://assets.mixkit.co/videos/1479/1479-1080.mp4",
        posterUrl: "https://assets.mixkit.co/videos/1479/1479-thumb-1080-0.jpg"
    },
    {
        sourceUrl: "https://assets.mixkit.co/videos/1778/1778-1080.mp4",
        posterUrl: "https://assets.mixkit.co/videos/1778/1778-thumb-1080-0.jpg"
    }
];

export const AnimalVideo = ({ index, isRandom }: AnimalVideoProps) => {
    
    const [selectedVideo, setSelectedVideo] = useState<VideoData | null>(null);

    useEffect(() => {
        if (isRandom && index === undefined) {
            const randomIndex = Math.floor(Math.random() * VIDEO_DATA.length);
            setSelectedVideo(VIDEO_DATA[randomIndex]);
        } else if (index !== undefined && !isRandom) {
            if (index >= 0 && index < VIDEO_DATA.length) {
                setSelectedVideo(VIDEO_DATA[index]);
            }
        }
    }, [isRandom, index]);

    return selectedVideo ? (
        <video
            controlsList="nodownload"
            poster={selectedVideo?.posterUrl}
            disablePictureInPicture
            autoPlay
            playsInline
            loop
            muted
        >
            <source  src={selectedVideo?.sourceUrl} type="video/mp4" />
        </video>     
    ) : null;
}