/*
 * Notifo.io
 *
 * @license
 * Copyright (c) Sebastian Stehle. All rights reserved.
 */

/** @jsx h */
import { Fragment, h } from 'preact';

import { NotificationsOptions, NotifoNotification, SDKConfig } from '@sdk/shared';
import { useStore } from '@sdk/ui/model';
import { useCallback } from 'preact/hooks';
import { Icon } from './Icon';
import { Loader } from './Loader';
import { NotificationItem } from './NotificationItem';

export interface NotificationsListProps {
    // The main config.
    config: SDKConfig;

    // The options.
    options: NotificationsOptions;

    // The html parent.
    parent?: HTMLElement;

    // Clicked when a notification is confirmed.
    onConfirm?: (notification: NotifoNotification) => Promise<any>;

    // Clicked when a notification is seen.
    onSeen?: (notification: NotifoNotification) => Promise<any>;

    // To toggle the profile view.
    onShowProfile?: (show: boolean) => void;
}

export const NotificationsList = (props: NotificationsListProps) => {
    const {
        config,
        onConfirm,
        onSeen,
        onShowProfile,
        options,
        parent,
    } = props;

    const notifications = useStore(x => x.notifications);
    const isLoaded = useStore(x => x.notificationsTransition !== 'InProgress');
    const isConnected = useStore(x => x.isConnected);

    const doShowProfile = useCallback((event: Event) => {
        onShowProfile && onShowProfile(true);

        event.preventDefault();
        event.stopImmediatePropagation();
        event.stopPropagation();
    }, [onShowProfile]);

    return (
        <Fragment>
            <button class='notifo-profile-button' type='button' onClick={doShowProfile}>
                <Icon type='settings' size={20} />
            </button>

            {!isConnected || !isLoaded &&
                <div class='notifo-loading'>
                    <Loader size={18} visible={true} />
                </div>
            }

            {isLoaded && (!isLoaded || notifications.length === 0) &&
                <div class='notifo-empty'>{config.texts.notificationsEmpty}</div>
            }

            {isLoaded && notifications.length > 0 &&
                <Fragment>
                    {notifications.map(x => (
                        <NotificationItem key={x.id}
                            config={config}
                            notification={x}
                            options={options}
                            onConfirm={onConfirm}
                            onSeen={onSeen}
                            modal={parent}
                        />
                    ))}
                </Fragment>
            }
        </Fragment>
    );
};
